
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_Issue:I_AllRepository<IssueT>
    {
        IEnumerable<IssueT> GetList(int repositoryId);
        bool Exist(int repositoryId,int issueNumber);
        IssueT GetByNumber(int repositoryId,int issueNumber);
        int GetIssueCount(int repoId);
        int GetPullRequestCount(int repoId);

        List<SentimentData> GetAllSentiment(int repoId);
        List<SentimentData> GetOnlySentiment(int repoId);
        List<SentimentData> GetAllSentiment(int repoId, int contributorId);
        List<SentimentData> GetOnlySentiment(int repoId, int contributorId);

        List<SentimentData> GetPullRequestAllSentiment(int repoId);
        List<SentimentData> GetPullRequestOnlySentiment(int repoId);
        List<SentimentData> GetPullRequestAllSentiment(int repoId, int contributorId);
        List<SentimentData> GetPullRequestOnlySentiment(int repoId, int contributorId);

        ReplyList<IssueView> GetFilterList(IssueFilter filter); // contain comment
        ReplyList<IssueView> GetPullRequestFilterList(IssueFilter filter); // contain comments


        List<SentimentData> GetFilterSentiment(IssueFilter filter);
        List<SentimentData> GetPullRequestFilterSentiment(IssueFilter filter);

        List<IssueT> GetRepositoryIssueList(int repoId);
        List<IssueT> GetRepositoryPullRequestList(int repoId);

        List<int> GetIssueNumberList(int repoId);
        List<int> GetPullRequestNumberList(int repoId);



    }
}
