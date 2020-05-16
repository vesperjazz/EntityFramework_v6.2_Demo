using System;

namespace EntityFrameworkDemo.Domain.DomainModels
{
    public class PersonContactNumber : EntityBase
    {
        public string PhoneNumber { get; set; }

        public Person Person { get; set; }
        public Guid PersonID { get; set; }
    }
}
