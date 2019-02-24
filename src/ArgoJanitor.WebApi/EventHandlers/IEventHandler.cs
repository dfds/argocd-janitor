using System;
using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.EventHandlers
{
    public interface IEventHandler<in T>
    {
        Task HandleAsync(T domainEvent);
    }
}