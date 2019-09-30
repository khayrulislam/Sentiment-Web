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
        public SentiDbContext() : base("name=SentiDbContext")
        {

        }

        public DbSet<UserData> Users { get; set; }
        public DbSet<RepositoryData> Repositories { get; set; }
        public DbSet<BranchData> Branches { get; set; }
        public DbSet<ContributorData> Contributors { get; set; }
        public DbSet<CommitData> Commits { get; set; }
        public DbSet<RepositoryContributorMap> RepositoryContributors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RepositoryContributorMap>()
            .HasKey(rcm => rcm.Id)
            .ToTable("RepositoryContributors");

            modelBuilder.Entity<RepositoryContributorMap>()
            .HasRequired(rcm => rcm.RepositoryData).WithMany(r => r.RepositoryMap)
            .HasForeignKey(rcm => rcm.RepositoryId);

            modelBuilder.Entity<RepositoryContributorMap>()
            .HasRequired(rcm => rcm.ContributorData).WithMany(c => c.ContributorMap)
            .HasForeignKey(rcm => rcm.ContributorId);

        }

    }

}
