using EventBus.Base;
using EventBus.Base.Abstraction;

namespace EventBus.Factory;

public static class EventBusFactory
{

    public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
    {
        switch (config.EventBusType)
        {
            case EventBusType.AzureServiceBus:
                return EventBusServiceBus(config, serviceProvider);

            case EventBusType.RabbitMQ:
                return EventBusRabbitMQ(config, serviceProvider);

            default:
                return null;
        }
    }

    private static IEventBus EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider)
    {
        return EventBusRabbitMQ(config, serviceProvider);
    }

    private static IEventBus EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider)
    {
        return EventBusRabbitMQ(config, serviceProvider);
    }
}

