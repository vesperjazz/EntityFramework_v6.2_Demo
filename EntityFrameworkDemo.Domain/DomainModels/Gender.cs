﻿using System.Collections.Generic;

namespace EntityFrameworkDemo.Domain.DomainModels
{
    public class Gender : EntityBase
    {
        public string Name { get; set; }

        public ICollection<Person> Persons { get; set; }
    }
}
