using System.Threading.Tasks;
using ArgoJanitor.WebApi.Domain.Events;

namespace ArgoJanitor.WebApi.Infrastructure.Messaging
{
    public interface IEventDispatcher
    {
        Task Send(string generalDomainEventJson);
        Task SendAsync(GeneralDomainEvent generalDomainEvent);
    }
    
}