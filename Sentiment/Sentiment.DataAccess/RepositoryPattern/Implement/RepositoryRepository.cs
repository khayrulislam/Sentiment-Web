﻿using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class RepositoryRepository: AllRepository<RepositoryData>, IRepositoryRepository
    {
        public RepositoryRepository(SentiDbContext dbContext): base(dbContext)
        {

        }
    }
}
