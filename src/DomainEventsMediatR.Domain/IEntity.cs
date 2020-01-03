using System;
using System.Collections.Concurrent;

namespace DomainEventsMediatR.Domain
{
    public interface IEntity
    {
        IProducerConsumerCollection<IDomainEvent> DomainEvents { get; }
    }
}
