using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Issue_:AllRepository<IssueT>,I_Issue
    {
        SentiDbContext _dbContext;
        public Issue_(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public bool Exist(int repositoryId, int issueNumber)
        {
            return _dbContext.Issues.Any(i=>i.RepositoryId == repositoryId && i.IssueNumber == issueNumber);
        }

        public IssueT GetByNumber(int repositoryId, int issueNumber)
        {
            return _dbContext.Issues.Where(i=>i .RepositoryId == repositoryId && i.IssueNumber == issueNumber).FirstOrDefault();
        }

        public int GetIssueCount(int repoId)
        {
            return _dbContext.Issues.Where(i => i.RepositoryId == repoId && i.IssueType==IssueType.Issue).Count();
        }

        public IEnumerable<IssueT> GetList(int repositoryId)
        {
            throw new NotImplementedException();
        }

        public int GetPullRequestCount(int repoId)
        {
            return _dbContext.Issues.Where(i => i.RepositoryId == repoId && i.IssueType == IssueType.PullRequest).Count();
        }
    }
}
