using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models
{
    public class MyDbContext:DbContext
    {
        public MyDbContext():base("DbConectionStringHome")
        {

        }

        public DbSet<People> Peoples { get; set; }
    }
}
