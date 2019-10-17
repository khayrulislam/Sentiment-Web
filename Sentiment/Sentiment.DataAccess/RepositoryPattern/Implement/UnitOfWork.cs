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
        public I_RepositoryContributor RepositoryContributor { get; private set; }
        public I_BranchCommit BranchCommit { get; private set; }
        public I_IssueComment IssueComment { get; private set; }
        public I_CommitComment CommitComment { get; private set; }
        public I_Issue Issue{ get; private set; }

        private SentiDbContext _dbContext;

        public UnitOfWork()
        {
            this._dbContext = new SentiDbContext();
            Branch = new Branch_(_dbContext);
            Commit = new Commit_(_dbContext);
            Contributor = new Contributor_(_dbContext);
            Repository = new Repository_(_dbContext);
            RepositoryContributor = new RepositoryContributor(_dbContext);
            BranchCommit = new BranchCommit(_dbContext);
            IssueComment = new IssueComment_(_dbContext);
            CommitComment = new CommitComment_(_dbContext);
            Issue = new Issue_(_dbContext);
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
