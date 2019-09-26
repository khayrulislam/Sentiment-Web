using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class UserRepository: AllRepository<User>, IUserRepository
    {
        public UserRepository(SentiDbContext dbContext):base(dbContext)
        {
        }

    }
}
