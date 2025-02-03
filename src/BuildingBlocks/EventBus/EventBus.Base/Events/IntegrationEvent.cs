using System.Text.Json.Serialization;

namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createdDate)
        {
            Id=id;
            CreatedDate = createdDate;
        }
    }
}
