﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface IUserRepository: IRepository<UserData>
    {
        bool UserExist(int userId);
    }
}