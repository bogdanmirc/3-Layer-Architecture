﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Person : BaseEntity
    {
        public string Name { get; set; } 
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }

        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

    }
}
