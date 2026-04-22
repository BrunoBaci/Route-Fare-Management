using System.ComponentModel.DataAnnotations.Schema;

namespace Route_Fare_Management.Domain
{
    [NotMapped]
    public class DomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
