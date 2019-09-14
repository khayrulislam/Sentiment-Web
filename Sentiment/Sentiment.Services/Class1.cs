using Sentiment.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services
{
    public class Class1
    {
        public List<User> GetUser()
        {
            using (var ctx = new SentiDbContext())
            {
                return ctx.Users.Select(u=>u).ToList();
            }
        }
    }
}
