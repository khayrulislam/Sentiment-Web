using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Interface
{
    public interface ICommit: IDisposable
    {
        void StoreCommit(Commit commit);
        List<Commit> GetCommits();
        Commit GetCommitById();

    }
}
