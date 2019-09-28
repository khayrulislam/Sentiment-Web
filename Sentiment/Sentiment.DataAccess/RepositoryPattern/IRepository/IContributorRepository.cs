﻿using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface IContributorRepository: IRepository<ContributorData>
    {
        //IEnumerable<ContributorData> GetRepositoryContributors(int repoId);

        bool ContributorExist(string contributorName);

        ContributorData GetContributor(string contributorName);
    }
}