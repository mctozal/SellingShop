using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using PaymentService.IntegrationEvents.EventHandlers;
using PaymentService.IntegrationEvents.Events;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// using logger for events
builder.Services.AddLogging(configure => configure.AddConsole());

// new instance needed for each request
builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();

builder.Services.AddSingleton<IEventBus>(serviceProvider =>
{
    EventBusConfig config = new EventBusConfig()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "PaymentService",
        EventBusType = EventBusType.RabbitMQ
    };
    return EventBusFactory.Create(config, serviceProvider);
});



var app = builder.Build();

// get event bus integrated with rabbit mq
IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();

// start listening event queue
eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
