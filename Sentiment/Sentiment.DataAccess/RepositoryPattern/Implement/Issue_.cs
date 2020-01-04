using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Issue_:AllRepository<IssueT>,I_Issue
    {
        SentiDbContext _dbContext;
        public Issue_(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public bool Exist(int repositoryId, int issueNumber)
        {
            return _dbContext.Issues.Any(i=>i.RepositoryId == repositoryId && i.IssueNumber == issueNumber);
        }

        public IssueT GetByNumber(int repositoryId, int issueNumber)
        {
            return _dbContext.Issues.Where(i=>i .RepositoryId == repositoryId && i.IssueNumber == issueNumber).FirstOrDefault();
        }

        public int GetIssueCount(int repoId)
        {
            return _dbContext.Issues.Where(i => i.RepositoryId == repoId && i.IssueType==IssueType.Issue).Count();
        }

        public IEnumerable<IssueT> GetList(int repositoryId)
        {
            throw new NotImplementedException();
        }

        public List<SentimentData> GetAllSentiment(int repoId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.Issue)
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public List<SentimentData> GetOnlySentiment(int repoId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.Issue && (iss.Pos != 1 || iss.Neg != -1))
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public List<SentimentData> GetAllSentiment(int repoId, int contributorId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.Issue && iss.WriterId == contributorId )
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public List<SentimentData> GetOnlySentiment(int repoId, int contributorId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType==IssueType.Issue && iss.WriterId == contributorId && (iss.Pos != 1 || iss.Neg != -1))
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value , Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public int GetPullRequestCount(int repoId)
        {
            return _dbContext.Issues.Where(i => i.RepositoryId == repoId && i.IssueType == IssueType.PullRequest).Count();
        }

        public List<SentimentData> GetPullRequestAllSentiment(int repoId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.PullRequest)
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public List<SentimentData> GetPullRequestOnlySentiment(int repoId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.PullRequest && (iss.Pos != 1 || iss.Neg != -1))
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public List<SentimentData> GetPullRequestAllSentiment(int repoId, int contributorId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.PullRequest && iss.WriterId == contributorId)
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public List<SentimentData> GetPullRequestOnlySentiment(int repoId, int contributorId)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.PullRequest && iss.WriterId == contributorId && (iss.Pos != 1 || iss.Neg != -1))
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public ReplyList<IssueView> GetFilterList(IssueFilter filter)
        {
            int total = 0;
            List<IssueView> list = new List<IssueView>();
            try
            {
                filter.SearchText = filter.SearchText.ToLower(); // no search text apply

                total = _dbContext.Issues
                        .Where(iss => iss.RepositoryId == filter.RepoId && iss.IssueType == IssueType.Issue
                        && ( (filter.Comment == "all") ? true : (filter.Comment == "only" ? iss.Comments.Count() > 0 : iss.Comments.Count() == 0) )
                        && ( (filter.State == "all") ? true : (filter.State == "open" ? iss.State == "open" : iss.State == "closed"))).Count();

                list = _dbContext.Issues
                        .Where(iss => iss.RepositoryId == filter.RepoId && iss.IssueType == IssueType.Issue
                        && ( (filter.Comment == "all") ? true : (filter.Comment == "only"? iss.Comments.Count() > 0 : iss.Comments.Count() == 0) )
                        && ( (filter.State == "all") ? true : (filter.State == "open" ? iss.State == "open" : iss.State == "closed")))
                        .Select(iss => new IssueView() { Id = iss.Id, IssueNumber = iss.IssueNumber, CommentCount = iss.Comments.Count(), UpdateDate = iss.UpdateDate, State = iss.State, Pos = iss.Pos, Neg = iss.Neg, NegTitle = iss.NegTitle, PosTitle = iss.PosTitle })
                        .OrderBy(iss => iss.UpdateDate)
                        .Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return new ReplyList<IssueView>
            {
                Data = list,
                TotalData = total
            };
        }

        public ReplyList<IssueView> GetPullRequestFilterList(IssueFilter filter)
        {
            int total = 0;
            List<IssueView> list = new List<IssueView>();
            try
            {
                filter.SearchText = filter.SearchText.ToLower(); // no search text apply
                total = _dbContext.Issues
                        .Where(iss => iss.RepositoryId == filter.RepoId && iss.IssueType == IssueType.PullRequest
                        && ((filter.Comment == "all") ? true : (filter.Comment == "only" ? iss.Comments.Count() > 0 : iss.Comments.Count() == 0))
                        && ((filter.State == "all") ? true : (filter.State == "open" ? iss.State == "open" : iss.State == "closed"))).Count();

                list = _dbContext.Issues
                        .Where(iss => iss.RepositoryId == filter.RepoId && iss.IssueType == IssueType.PullRequest
                        && ( (filter.Comment == "all") ? true : (filter.Comment == "only" ? iss.Comments.Count() > 0 : iss.Comments.Count() == 0))
                        && ( (filter.State == "all") ? true : (filter.State == "open" ? iss.State == "open" : iss.State == "closed")))
                        .Select(iss => new IssueView() { Id = iss.Id, IssueNumber = iss.IssueNumber, CommentCount = iss.Comments.Count(), UpdateDate = iss.UpdateDate, State = iss.State, Pos = iss.Pos, Neg = iss.Neg, NegTitle = iss.NegTitle, PosTitle = iss.PosTitle })
                        .OrderBy(iss => iss.UpdateDate)
                        .Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return new ReplyList<IssueView>
            {
                Data = list,
                TotalData = total
            };
        }

        public List<SentimentData> GetFilterSentiment(IssueFilter filter)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues.Where(iss => iss.RepositoryId == filter.RepoId && iss.IssueType == IssueType.Issue
                    && ((filter.Comment == "all") ? true : (filter.Comment == "only" ? iss.Comments.Count() > 0 : iss.Comments.Count() == 0))
                    && ((filter.State == "all") ? true : (filter.State == "open" ? iss.State == "open" : iss.State == "closed")))
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); throw;
            }
            return list;
        }

        public List<SentimentData> GetPullRequestFilterSentiment(IssueFilter filter)
        {
            List<SentimentData> list = new List<SentimentData>();
            try
            {
                list = _dbContext.Issues
                    .Where(iss => iss.RepositoryId == filter.RepoId && iss.IssueType == IssueType.PullRequest
                    && ((filter.Comment == "all") ? true : (filter.Comment == "only" ? iss.Comments.Count() > 0 : iss.Comments.Count() == 0))
                    && ((filter.State == "all") ? true : (filter.State == "open" ? iss.State == "open" : iss.State == "closed")))
                    .OrderBy(iss => new { iss.UpdateDate, iss.Id })
                    .Select(iss => new SentimentData() { Datetime = iss.UpdateDate.Value, Neg = iss.Neg, Pos = iss.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); throw;
            }
            return list;
        }

        public List<IssueT> GetRepositoryIssueList(int repoId)
        {
            List<IssueT> list = new List<IssueT>();
            try
            {
                list = _dbContext.Issues.Include("Comments").Include("Creator").AsNoTracking().Where(iss=>iss.RepositoryId == repoId && iss.IssueType==IssueType.Issue && iss.State == "closed").ToList(); // colsed issue
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        public List<IssueT> GetRepositoryPullRequestList(int repoId)
        {
            List<IssueT> list = new List<IssueT>();
            try
            {
                list = _dbContext.Issues.AsNoTracking().Where(iss => iss.RepositoryId == repoId && iss.IssueType == IssueType.PullRequest).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }
    }
}
