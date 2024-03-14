using domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataOfSql
{
    public class personcontext : DbContext
    {
        public personcontext(DbContextOptions<personcontext> options)
              : base(options)
        {

        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> PersonAddress { get; set; }
    }
}
