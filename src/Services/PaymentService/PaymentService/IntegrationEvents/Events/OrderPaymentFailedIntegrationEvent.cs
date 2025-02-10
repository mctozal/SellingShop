using EventBus.Base.Events;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace PaymentService.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public string ErrorMessage { get; }

        public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
        {
            OrderId = orderId;
            ErrorMessage = errorMessage;
        }
    }
}
