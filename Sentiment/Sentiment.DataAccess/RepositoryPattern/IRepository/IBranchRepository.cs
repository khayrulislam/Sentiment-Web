﻿using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface IBranchRepository:IRepository<BranchData>
    {
        IEnumerable<BranchData> GetRepositoryBranches(int repoId);
    }
}