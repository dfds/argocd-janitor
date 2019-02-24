using System;

namespace ArgoJanitor.WebApi.Domain.Events
{
    public interface IDomainEvent<T>
    {
        Guid MessageId { get; }
        string Type { get; }
        T Data { get; }
    }
}