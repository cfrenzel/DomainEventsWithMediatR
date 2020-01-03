using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DomainEventsMediatR.Domain;

namespace DomainEventsMediatR.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public DbSet<BacklogItem> BacklogItems { get; set; }
        public DbSet<Sprint> Sprints { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
         IDomainEventDispatcher dispatcher)
         : base(options)
        {
            _dispatcher = dispatcher;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Sprint>(s =>
            {
                s.HasMany(e => e.BackLogItems)
                  .WithOne(e => e.Sprint)
                  .OnDelete(DeleteBehavior.SetNull);
            });

        }

        public override int SaveChanges()
        {
            _preSaveChanges().GetAwaiter().GetResult();
            var res = base.SaveChanges();
            return res;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _preSaveChanges();
            var res = await base.SaveChangesAsync(cancellationToken);
            return res;
        }

        private async Task _preSaveChanges()
        {
            await _dispatchDomainEvents();
        }

        /// <summary>
        /// Domain events run within the transaction and 
        /// allow aggregates to locally communicate with eachother
        /// cleanly
        /// </summary>
        private async Task _dispatchDomainEvents()
        {
            var domainEventEntities = ChangeTracker.Entries<IEntity>()
               .Select(po => po.Entity)
               .Where(po => po.DomainEvents.Any())
               .ToArray();

            foreach (var entity in domainEventEntities)
            {
                IDomainEvent dev;
                while (entity.DomainEvents.TryTake(out dev))
                    await _dispatcher.Dispatch(dev);
            }
        }

    }


}


