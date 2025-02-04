using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;
using EventBus.AzureServiceBus;


namespace EventBus.Factory;

public static class EventBusFactory
{

    public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
    {
        switch (config.EventBusType)
        {
            case EventBusType.AzureServiceBus:
                return new EventBusServiceBus(serviceProvider, config);

            case EventBusType.RabbitMQ:
                var eventBus = new EventBusRabbitMQ(serviceProvider, config);

                return eventBus;

            default:
                return null;
        }
    }

}

