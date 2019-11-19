﻿using Sentiment.DataAccess.DataClass;
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
    }
}
