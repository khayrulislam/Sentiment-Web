using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
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

        /*public bool Exist(long issueId)
        {
            return _dbContext.Issues.Any(i=>i.IssueId == issueId);
        }*/

        public bool Exist(int repositoryId, long issueId)
        {
            return _dbContext.Issues.Any(i=>i.RepositoryId == repositoryId && i.IssueNumber == issueId);
        }

        public IEnumerable<IssueT> GetList(int repositoryId)
        {
            return _dbContext.Issues.Where(i => i.RepositoryId == repositoryId).ToList();
        }
    }
}
