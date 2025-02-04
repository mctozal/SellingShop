using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory connectionFactory;
        private  IChannel consumerChannel;

        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
        {
            if (eventBusConfig.Connection != null)
            {
                var connectionJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
                {
                    // Self referencing loop detected for property
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connectionJson);
            }
            else connectionFactory = new ConnectionFactory();

            persistentConnection = new RabbitMQPersistentConnection(connectionFactory, EventBusConfig.ConnectionRetryCount);
            
            consumerChannel = CreateConsumerChannel().GetAwaiter().GetResult();

            SubscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
        }

        public override async void PublishAsync(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, time) =>
                {
                    // ToDo: log
                });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            // Ensure exchange exists while publishing
            await consumerChannel.ExchangeDeclareAsync(exchange: EventBusConfig.DefaultTopicName, type: "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };


            policy.Execute( () =>
            {
          
                // Ensure if queue exists while publishing
                //consumerChannel.QueueDeclare(
                //        queue: GetSubName(eventName),
                //        durable: true,
                //        exclusive: false,
                //        autoDelete: false,
                //        arguments: null);

                //consumerChannel.QueueBind(
                //        queue: GetSubName(eventName),
                //        exchange: EventBusConfig.DefaultTopicName,
                //        routingKey: eventName);

                 consumerChannel.BasicPublishAsync(
                        exchange: EventBusConfig.DefaultTopicName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body).GetAwaiter().GetResult();
            });

        }

        public override async void SubscribeAsync<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                // Ensure if queue exists while consuming
                await consumerChannel.QueueDeclareAsync(queue: GetSubName(eventName),
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                await consumerChannel.QueueBindAsync(queue: GetSubName(eventName),
                                        exchange: EventBusConfig.DefaultTopicName,
                                        routingKey: eventName);
            }

            SubscriptionManager.AddSubscription<T, TH>();

            StartBasicConsume(eventName);
        }

        public override void UnSubscribe<T, TH>()
        {
            SubscriptionManager.RemoveSubscription<T, TH>();
        }

        private async void SubscriptionManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            await consumerChannel.QueueUnbindAsync(
                    queue: eventName,
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName
                );

            if (SubscriptionManager.IsEmpty)
            {
                await consumerChannel.CloseAsync();
            }
        }

        private async Task<IChannel?> CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = await persistentConnection.CreateModel();

             await channel.ExchangeDeclareAsync(exchange: EventBusConfig.DefaultTopicName,
                                    type: "direct");

            return channel;
        }

        private async void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new /*Async*/AsyncEventingBasicConsumer(consumerChannel);

                consumer.ReceivedAsync += Consumer_Received;

               await consumerChannel.BasicConsumeAsync(
                        queue: GetSubName(eventName),
                        autoAck: false,
                        consumer: consumer
                    );
            }
        }

        private async Task Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                // ToDo: log
            }

            await consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
    }
}
