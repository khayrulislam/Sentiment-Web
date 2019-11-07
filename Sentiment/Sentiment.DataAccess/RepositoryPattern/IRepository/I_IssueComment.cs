using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_IssueComment:I_AllRepository<IssueCommentT>
    {
        bool Exist(int issueId,int commentNumber);
        IssueCommentT GetByNumber(int issueId, long commentNumber);
        int GetIssueCommentCount(int repoId);
        int GetPullRequestCommentCount(int repoId);
    }
}
