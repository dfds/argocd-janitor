using System;

namespace ArgoJanitor.WebApi.Domain.Events
{
    public class GeneralDomainEvent : IDomainEvent<object>
    {
        public Guid MessageId { get; private set; }

        public string Type { get; private set; }

        public object Data { get; private set; }

        public GeneralDomainEvent(
            Guid messageId,
            string type,
            object data)
        {
            MessageId = messageId;
            Type = type;
            Data = data;
        }
    }
}