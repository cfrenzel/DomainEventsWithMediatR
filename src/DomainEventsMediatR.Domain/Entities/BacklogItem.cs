using System;
using System.ComponentModel.DataAnnotations;

namespace DomainEventsMediatR.Domain
{
    public class BacklogItem : Entity
    {
        public Guid Id { get; private set; }

        [MaxLength(255)]
        public string Description { get; private set; }
        public virtual Sprint Sprint { get; private set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        private BacklogItem() { }

        public BacklogItem(string desc)
        {
            this.Id = NewIdGuid();
            this.Description = desc;
        }
    
        public void CommitTo(Sprint s)
        {
            this.Sprint = s;
            this.PublishEvent(new BacklogItemCommitted(this, s));
        }
    }
}
