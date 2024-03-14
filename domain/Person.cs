using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace domain
{
    public class Person
    {
        public int PersonId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreateTime { get; set; }    
        public string? FirstName { get; set; }
        public string? userName {  get; set; }
        public string? password { get; set; }
        public string? email { get; set; }   
        public string? LastName { get; set; }    
        public string? JobPosition { get; set; }
        public double? Salary { get; set; } = null;
        public double? WorkExperience { get; set; }
        public Address PersonAddress { get; set; }
        public string? Role { get; set; }
    }
}
