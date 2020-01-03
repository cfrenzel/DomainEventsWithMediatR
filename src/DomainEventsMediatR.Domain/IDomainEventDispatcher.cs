using System.Threading.Tasks;

namespace DomainEventsMediatR.Domain
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(IDomainEvent devent);
    }
}
