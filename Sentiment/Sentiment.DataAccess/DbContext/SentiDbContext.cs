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

        public DbSet<UserT> Users { get; set; }
        public DbSet<RepositoryT> Repositories { get; set; }
        public DbSet<BranchT> Branches { get; set; }
        public DbSet<ContributorT> Contributors { get; set; }
        public DbSet<CommitT> Commits { get; set; }
        public DbSet<RepositoryContributorT> RepositoryContributors { get; set; }
        public DbSet<BranchCommitT> BranchCommits { get; set; }
        public DbSet<PullRequestT> PullRequests{ get; set; }
        public DbSet<CommentT> Comments{ get; set; }
        public DbSet<IssueT> Issues{ get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RepositoryContributorT>()
            .HasKey(rcm => rcm.Id)
            .ToTable("RepositoryContributor");

            modelBuilder.Entity<RepositoryContributorT>()
            .HasRequired(rcm => rcm.Repository).WithMany(r => r.RepositoryContributors)
            .HasForeignKey(rcm => rcm.RepositoryId);

            modelBuilder.Entity<RepositoryContributorT>()
            .HasRequired(rcm => rcm.Contributor).WithMany(c => c.RepositoryContributors)
            .HasForeignKey(rcm => rcm.ContributorId);



            modelBuilder.Entity<BranchCommitT>()
            .HasKey(bc => bc.Id)
            .ToTable("BranchCommit");

            modelBuilder.Entity<BranchCommitT>()
            .HasRequired(bc => bc.Branch).WithMany(b => b.BranchCommits)
            .HasForeignKey(bc => bc.BranchId);

            modelBuilder.Entity<BranchCommitT>()
            .HasRequired(bc => bc.Commit).WithMany(c => c.BranchCommits)
            .HasForeignKey(bc => bc.CommitId);
        }

    }

}
