using EventBus.Base.Events;

namespace PaymentService.IntegrationEvents.Events
{
    public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
    {

        public int OrderId { get; }

        public OrderPaymentSuccessIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}
