using Sentiment.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Interface
{
    public interface IUser: IDisposable
    {
        void StoreUser(User user);

        List<User> GetUsers();

        User GetUserById();



    }
}
