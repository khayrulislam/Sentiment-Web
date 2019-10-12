using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class UnitOfWork : IUnitOfWork
    {
        public IBranchRepository Branch { get; private set; }

        public ICommitRepository Commit { get; private set; }

        public IContributorRepository Contributor { get; private set; }

        public IRepositoryRepository Repository { get; private set; }

        public IUserRepository User { get; private set; }

        public IRepositoryContributor RepositoryContributor { get; private set; }

        public IBranchCommit BranchCommit { get; private set; }

        private SentiDbContext _dbContext;

        public UnitOfWork(SentiDbContext dbContext)
        {
            this._dbContext = dbContext;
            Branch = new BranchRepository(_dbContext);
            Commit = new CommitRepository(_dbContext);
            Contributor = new ContributorRepository(_dbContext);
            Repository = new RepositoryRepository(_dbContext);
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
