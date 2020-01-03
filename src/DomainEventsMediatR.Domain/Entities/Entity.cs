using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainEventsMediatR.Domain
{ 
    public abstract class Entity : IEntity
    {
        
        [NotMapped]
        private readonly ConcurrentQueue<IDomainEvent> _domainEvents = new ConcurrentQueue<IDomainEvent>();

        [NotMapped]
        public IProducerConsumerCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void PublishEvent(IDomainEvent @event)
        {
            _domainEvents.Enqueue(@event);
        }

        protected Guid NewIdGuid()
        {
            return MassTransit.NewId.NextGuid();
        }

    }
}
