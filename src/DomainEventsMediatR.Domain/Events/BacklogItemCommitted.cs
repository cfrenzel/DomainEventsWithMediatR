using System;

namespace DomainEventsMediatR.Domain
{
    public class BacklogItemCommitted : IDomainEvent
    {
        public Guid BacklogItemId { get; }
        public Guid SprintId { get; set; }
        public DateTime CreatedAtUtc { get; }

        private BacklogItemCommitted() { }

        public BacklogItemCommitted(BacklogItem b, Sprint s)
        {
            this.BacklogItemId = b.Id;
            this.CreatedAtUtc = b.CreatedAtUtc;
            this.SprintId = s.Id;
        }    
    }
}
