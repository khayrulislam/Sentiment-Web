using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.Shared;
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
        int GetCount(int repoId);
        int GetPullRequestCount(int repoId);


    }
}
