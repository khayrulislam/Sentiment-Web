using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Comment_:AllRepository<CommentT>,I_Comment
    {
        SentiDbContext _dbContext;
        public Comment_(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
