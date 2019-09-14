using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess
{
    public class SentiDbContext : DbContext
    {
        public SentiDbContext(): base("name=SentiDbContext")
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
