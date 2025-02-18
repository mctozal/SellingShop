using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace BasketService.Api.Extensions
{
    public static class EventRegistration
    {

        public static IServiceCollection ConfigureSubscription(this IServiceCollection services)
        {
            services.AddTransient<OrderCreatedIntegrationEventHandler>();
            return services;
        }

        public static IApplicationBuilder RegisterSubscription(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();

            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            return app;
        }
    }
}
