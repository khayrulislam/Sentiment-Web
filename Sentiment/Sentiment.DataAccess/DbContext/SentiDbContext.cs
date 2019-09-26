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

        public DbSet<UserData> Users { get; set; }
        public DbSet<RepositoryData> Repositories { get; set; }
        public DbSet<BranchData> Branches { get; set; }
        public DbSet<ContributorData> Contributors { get; set; }
        public DbSet<CommitData> Commits { get; set; }

    }
}
