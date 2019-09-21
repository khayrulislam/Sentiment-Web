using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Interface
{
    public interface IBranch: IDisposable
    {
        void StoreBranch(Branch branch);
        List<Branch> GetBranches();
        Branch GetBranchById();

    }
}
