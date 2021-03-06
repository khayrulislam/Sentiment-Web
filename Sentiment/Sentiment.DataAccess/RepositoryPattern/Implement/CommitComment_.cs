﻿using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class CommitComment_: AllRepository<CommitCommentT>,I_CommitComment
    {
        SentiDbContext _dbContext;
        public CommitComment_(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public bool Exist(int commitId, long commentNumber)
        {
            return _dbContext.CommitComments.Any(cc=>cc.CommitId == commitId && cc.CommentNumber == commentNumber);
        }

        public CommitCommentT GetByNumber(int commitId, long commentNumber)
        {
            return _dbContext.CommitComments.Where(cc=>cc.CommitId == commitId && cc.CommentNumber == commentNumber).FirstOrDefault();
        }

        public int GetCount(int repoId)
        {
            return _dbContext.Commits.Where(c=> c.RepositoryId == repoId && c.Comments.Any(cc=>cc.CommitId == c.Id)).Count();
        }

        public List<CommitCommentT> GetList(int repoId)
        {
            return _dbContext.CommitComments.AsNoTracking().Where(cc=>cc.Commit.RepositoryId == repoId).ToList();
        }
    }
}
