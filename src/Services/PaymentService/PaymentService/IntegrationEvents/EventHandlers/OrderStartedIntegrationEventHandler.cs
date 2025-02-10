using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.IntegrationEvents.Events;

namespace PaymentService.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IConfiguration configuration;
        private readonly IEventBus eventBus;
        private readonly ILogger<OrderStartedIntegrationEventHandler> logger;

        public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            this.configuration = configuration;
            this.eventBus = eventBus;
            this.logger = logger;
        }
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            // Fake payment process
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = configuration.GetValue<bool>(keyword);

            IntegrationEvent paymentEvent = paymentSuccessFlag 
                ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
                : new OrderPaymentFailedIntegrationEvent(@event.OrderId,"This is fake error message");

            logger.LogInformation($"OrderCreatedEventHandler in PaymentService is fired with PaymentSuccess : {paymentSuccessFlag}, orderId : {@event.OrderId} ");

            
            eventBus.Publish(paymentEvent);
         
            return Task.CompletedTask;
        }
    }
}