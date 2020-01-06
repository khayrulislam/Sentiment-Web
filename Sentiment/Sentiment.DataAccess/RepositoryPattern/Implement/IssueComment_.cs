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
    public class IssueComment_:AllRepository<IssueCommentT>,I_IssueComment
    {
        SentiDbContext _dbContext;
        public IssueComment_(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public bool Exist(int issueId, int commentNumber)
        {
            return _dbContext.IssueComments.Any(ic=>ic.IssueId == issueId && ic.CommentNumber == commentNumber);
        }

        public IssueCommentT GetByNumber(int issueId, long commentNumber)
        {
            return _dbContext.IssueComments.Where(ic=> ic.IssueId == issueId && ic.CommentNumber == commentNumber).FirstOrDefault();
        }

        public int GetCount(int repoId)
        {
            return _dbContext.Issues.Where(i => i.RepositoryId == repoId && i.IssueType == IssueType.Issue ).Select(i => i.Comments.Count()).DefaultIfEmpty(0).Sum();
        }

        public List<IssueCommentT> GetIssueCommentList(int repoId)
        {
            return _dbContext.IssueComments.Include("Issue").Include("Creator").AsNoTracking().Where(iss => iss.RepositoryId == repoId && iss.Issue.IssueType == IssueType.Issue && iss.Issue.State == "closed").ToList(); // close issue commment

        }

        public List<IssueCommentT> GetPullRequestCommentList(int repoId)
        {
            return _dbContext.IssueComments.Include("Issue").Include("Creator").AsNoTracking().Where(iss => iss.RepositoryId == repoId && iss.Issue.IssueType == IssueType.PullRequest && iss.Issue.State == "closed").ToList(); // close pull request commment
        }

        public int GetPullRequestCount(int repoId)
        {
            return _dbContext.Issues.Where(i => i.RepositoryId == repoId && i.IssueType == IssueType.PullRequest ).Select(i=> i.Comments.Count()).DefaultIfEmpty(0).Sum();
        }
    }
}
