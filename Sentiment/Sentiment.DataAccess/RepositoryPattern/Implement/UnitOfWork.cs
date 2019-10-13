using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class UnitOfWork : I_UnitOfWork
    {
        public I_Branch Branch { get; private set; }

        public I_Commit Commit { get; private set; }

        public I_ContributorRepository Contributor { get; private set; }

        public I_Repository Repository { get; private set; }

        public I_User User { get; private set; }

        public I_RepositoryContributor RepositoryContributor { get; private set; }

        public I_BranchCommit BranchCommit { get; private set; }

        private SentiDbContext _dbContext;

        public UnitOfWork(SentiDbContext dbContext)
        {
            this._dbContext = dbContext;
            Branch = new Branch_(_dbContext);
            Commit = new Commit_(_dbContext);
            Contributor = new Contributor_(_dbContext);
            Repository = new Repository_(_dbContext);
            User = new UserRepository(_dbContext);
            RepositoryContributor = new RepositoryContributor(_dbContext);
            BranchCommit = new BranchCommit(_dbContext);
        }

        public int Complete()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
