using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Application.Services.Abstract;
using BasketService.Application.Services.Concrete;
using BasketService.Extensions;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.ConfigureConsul(builder.Configuration);

builder.Services.ConfigureRedis(builder.Configuration);

builder.Services.AddRedis(builder.Configuration);

builder.Services.ConfigureSubscription();

builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();

builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddSingleton<IEventBus>(serviceProvider =>
{
    EventBusConfig config = new EventBusConfig()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ
    };
    return EventBusFactory.Create(config, serviceProvider);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.RegisterConsul(app.Services.GetRequiredService<IHostApplicationLifetime>(), builder.Configuration);

app.RegisterSubscription();

app.Run();
