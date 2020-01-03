using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using DomainEventsMediatR.Domain;
using DomainEventsMediatR.Persistence;

namespace DomainEventsMediatR.Application
{
    public class OnBacklogItemCommitted
    {
        public class Handler : INotificationHandler<DomainEventNotification<BacklogItemCommitted>>
        {
            private readonly ApplicationDbContext _db;
            private readonly ILogger<Handler> _log;
        
            public Handler(ApplicationDbContext db,  ILogger<Handler> log)
            {
                _db = db;
                _log = log;
            }

            public Task Handle(DomainEventNotification<BacklogItemCommitted> notification, CancellationToken cancellationToken)
            {
                var domainEvent = notification.DomainEvent;
                try
                {
                    _log.LogDebug("Handling Domain Event. BacklogItemId: {itemId}  Type: {type}", domainEvent.BacklogItemId, notification.GetType());
                    //from here you could 
                    // - create/modify entities within the same transactions that committed the backlogItem
                    // - trigger the publishing of an integrtion event on a servicebus (don't write it directly though, you need an outbox scoped to this transaction)
                                      
                    //Remember NOT to call SaveChanges on dbcontext if making db changes when handling DomainEvents
                    return Task.CompletedTask;
                }
                catch (Exception exc)
                {
                    _log.LogError(exc, "Error handling domain event {domainEvent}", domainEvent.GetType());
                    throw;
                }
            }
        }

    }
}
