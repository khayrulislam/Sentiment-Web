using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_CommitComment:I_AllRepository<CommitCommentT>
    {
        bool Exist(int commitId, long commentNumber);
        CommitCommentT GetByNumber(int commitId, long commentNumber);
        int GetCount(int repoId);
    }
}
