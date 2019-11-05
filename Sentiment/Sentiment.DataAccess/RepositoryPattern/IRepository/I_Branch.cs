using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_Branch:I_AllRepository<BranchT>
    {
        IEnumerable<BranchT> GetList(int repoId);
        Reply<BranchView> GetFilterList(BranchFilter filter);
        int GetCount(int repoId);

    }
}
