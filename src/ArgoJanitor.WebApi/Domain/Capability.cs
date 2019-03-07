using System;

namespace ArgoJanitor.WebApi.Domain
{
    public class Capability
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string AzureADObjectId { get; private set; }

        public Capability(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public static Capability Create(Guid id, string name)
        {
            var capability = new Capability(
                id: id,
                name: name
            );

            return capability;
        }
    }
}