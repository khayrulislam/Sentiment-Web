using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.Shared;
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
        int GetCount(int repoId);
        List<SentimentData> GetOnlySentiment(int repoId);
        List<SentimentData> GetAllSentiment(int repoId);
        List<SentimentData> GetOnlySentiment(int repoId, int contributorId);
        List<SentimentData> GetAllSentiment(int repoId, int contributorId);
        List<SentimentData> GetContributorAllSentiment(int repoId, int contributorId);
        List<CommitT> GetRepositoryCommitList(int repoId);

    }
}
