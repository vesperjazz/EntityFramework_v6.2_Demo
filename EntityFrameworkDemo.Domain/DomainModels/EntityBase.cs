using System;

namespace EntityFrameworkDemo.Domain.DomainModels
{
    public abstract class EntityBase
    {
        public Guid ID { get; set; }

        public EntityBase()
        {
            ID = Guid.NewGuid();
        }
    }
}
