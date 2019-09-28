using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class UserRepository: AllRepository<UserData>, IUserRepository
    {
        SentiDbContext _dbContext;
        public UserRepository(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public bool UserExist(int userId)
        {
            return _dbContext.Users.Any(u => u.Id == userId);
        }
    }
}
