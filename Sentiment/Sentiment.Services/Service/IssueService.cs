using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.Services.GitHub;
using Sentiment.Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class IssueService
    {
        GitHubClient gitHubClient;
        IIssuesClient issueClient;
        SentimentCal sentimentCal;
        RepositoryIssueRequest request;
        ApiOptions option;
        ContributorService contributorService;
        public IssueService()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.gitHubClient = GitHubConnection.Instance;
            this.issueClient = gitHubClient.Issue;
            this.sentimentCal = SentimentCal.Instance;
            this.contributorService = new ContributorService();

            this.request = new RepositoryIssueRequest()
            {
                State = ItemStateFilter.All
            };
            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 300
            };
        }

        public async Task StoreAllIssuesAsync(long repoId, int repositoryId)
        {
            int sPage = 0;
            while (true)
            {
                option.StartPage = ++sPage;
                var issueBlock = await issueClient.GetAllForRepository(repoId, request, option);
                if (issueBlock.Count == 0) break;
                else StoreIssueBlock(repositoryId, issueBlock);
            }
        }

        private void StoreIssueBlock(int repositoryId, IReadOnlyList<Issue> issueBlock)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                if (repositoryId != 0)
                {
/*                    var storedIssue = (List<IssueT>) unitOfWork.Issue.GetList(repositoryId);
                    if(storedIssue.Count > 0)
                    {
                        allIssues = allIssues.Where(i => !storedIssue.Any(si => si.IssueNumber == i.Number)).ToList();
                    }*/
                    var issueList = new List<IssueT>();
                    var pullRequestList = new List<PullRequestT>();

                    foreach (var issue in issueBlock)
                    {
                        sentimentCal.CalculateSentiment(issue.Body);
                        var issuer = contributorService.GetContributor(issue.User.Login);
                        if (issue.PullRequest == null)
                        {
                            if (!unitOfWork.Issue.Exist(issue.Id))
                            {
                                issueList.Add(new IssueT()
                                {
                                    RepositoryId = repositoryId,
                                    IssueNumber = issue.Number,
                                    Title = issue.Title,
                                    PosSentiment = sentimentCal.PositoiveSentiScore,
                                    NegSentiment = sentimentCal.NegativeSentiScore,
                                    WriterId = issuer.Id,
                                    State = issue.State.StringValue,
                                    IssueId = issue.Id
                                });
                            }
                        }
                        else
                        {
                            if (!unitOfWork.PullRequest.Exist(issue.Id))
                            {
                                pullRequestList.Add(new PullRequestT()
                                {
                                    RepositoryId = repositoryId,
                                    RequestNumber = issue.Number,
                                    Title = issue.Title,
                                    PosSentiment = sentimentCal.PositoiveSentiScore,
                                    NegSentiment = sentimentCal.NegativeSentiScore,
                                    WriterId = issuer.Id,
                                    State = issue.State.StringValue,
                                    PullRequestId = issue.Id
                                });
                            }
                        }
                    }
                    unitOfWork.Issue.AddRange(issueList);
                    unitOfWork.PullRequest.AddRange(pullRequestList);
                    unitOfWork.Complete();
                }
            }
        }
    }
}
