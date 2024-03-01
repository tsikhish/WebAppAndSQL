using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain
{
    public class Address
    {
        public int Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; }= string.Empty;
        public int HomeNumber { get; set; }
        public int PersonId {  get; set; }
    }
}
