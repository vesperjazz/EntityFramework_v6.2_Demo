using System;
using System.Collections.Generic;

namespace EntityFrameworkDemo.Domain.DomainModels
{
    public class Person : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; }
        // GenderID is optional, therefore it is important for the 
        // value type Guid to be nullable.
        public Guid? GenderID { get; set; }

        public virtual ICollection<PersonContactNumber> PersonContactNumbers { get; set; }
    }
}
