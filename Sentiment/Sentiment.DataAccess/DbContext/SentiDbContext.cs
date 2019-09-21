using Sentiment.DataAccess.DataClass;
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
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Commit> Commits { get; set; }

    }
}
