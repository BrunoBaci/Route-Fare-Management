using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }

        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents
            => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent e) => _domainEvents.Add(e);
        public void ClearDomainEvents() => _domainEvents.Clear();
        public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
    }
}
