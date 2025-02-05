using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.UnitTest
{
    [TestClass]
    public sealed class EventBusTests
    {

        private ServiceCollection services;

        public EventBusTests()
        {
            this.services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());
        }

        [TestMethod]
        public void subscribe_event_on_rabbitmq_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "UnitTestTopicName",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNamePrefix = "IntegrationEvent",
                };
                
                return EventBusFactory.Create(config, sp);
            });

            var serviceProvider = services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        }

        //[TestMethod]
        //public void subscribe_event_on_azure_test()
        //{
        //    services.AddSingleton<IEventBus>(sp =>
        //    {
        //        EventBusConfig config = new()
        //        {
        //            ConnectionRetryCount = 5,
        //            SubscriberClientAppName = "EventBus.UnitTest",
        //            DefaultTopicName = "UnitTestTopicName",
        //            EventBusType = EventBusType.AzureServiceBus,
        //            EventNamePrefix = "IntegrationEvent",
        //            EventBusConnectionString="***"
        //            //Connection = new ConnectionFactory()
        //            //{
        //            //    HostName = "localhost",
        //            //    Port = 15672,
        //            //    UserName = "guest",
        //            //    Password = "guest"
        //            //}
        //        };

        //        return EventBusFactory.Create(config, sp);
        //    });

        //    var serviceProvider = services.BuildServiceProvider();
        //    var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        //    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        //    eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        //}

        [TestMethod]
        public void send_message_to_rabbitmq()
        {

            EventBusConfig config = new()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "UnitTestTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNamePrefix = "IntegrationEvent",
            };

            services.AddSingleton<IEventBus>(
                sp => {
                    return EventBusFactory.Create(config, sp);
                });
            
            var serviceProvider = services.BuildServiceProvider();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            
            eventBus.Publish(new OrderCreatedIntegrationEvent(1));


        }
    }
}
