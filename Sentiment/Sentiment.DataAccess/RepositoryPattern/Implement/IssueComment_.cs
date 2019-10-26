using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
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
    }
}
