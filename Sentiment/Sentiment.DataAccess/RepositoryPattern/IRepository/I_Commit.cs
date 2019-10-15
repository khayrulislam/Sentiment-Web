using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_Commit:I_AllRepository<CommitT>
    {
        bool Exist(string sha);
        CommitT GetBySha(string sha);
    }
}
