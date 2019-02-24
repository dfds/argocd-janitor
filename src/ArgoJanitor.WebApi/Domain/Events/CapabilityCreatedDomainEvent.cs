using System;
using Newtonsoft.Json.Linq;

namespace ArgoJanitor.WebApi.Domain.Events
{
    public class CapabilityCreatedDomainEvent : IDomainEvent<CapabilityCreatedData>
    {
        public Guid MessageId { get; private set; }

        public string Type { get; private set; }

        public CapabilityCreatedData Data { get; private set; }

        public CapabilityCreatedDomainEvent(GeneralDomainEvent domainEvent)
        {
            MessageId = domainEvent.MessageId;
            Type = domainEvent.Type;
            Data = (domainEvent.Data as JObject)?.ToObject<CapabilityCreatedData>();
        }
    }

    public class CapabilityCreatedData
    {
        public Guid CapabilityId { get; private set; }
        public string CapabilityName { get; private set; }

        public CapabilityCreatedData(Guid capabilityId, string capabilityName)
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
        }
    }
}