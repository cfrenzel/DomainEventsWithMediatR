using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DomainEventsMediatR.Domain
{
    public class Sprint : Entity
    {
        public Guid Id { get; private set; }
        public DateTime StartDateUtc { get; private set; }
        public DateTime EndDateUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        public virtual ICollection<BacklogItem> BackLogItems { get; private set; } = new List<BacklogItem>();

        private Sprint() { }

        public Sprint(DateTime startUtc, DateTime endUtc)
        {
            this.Id = NewIdGuid();
            this.StartDateUtc = startUtc;
            this.EndDateUtc = endUtc;
        }

        public void AddBacklogItem(BacklogItem b)
        {
            this.BackLogItems.Add(b);
        }
    }
}
