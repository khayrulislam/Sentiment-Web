﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.Shared
{
    public class Filter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; }
        public string SortOrder { get; set; }
    }

    public class RepositroyFilter: Filter
    {

    }

    public class BranchFilter: Filter
    {
        public int Id { get; set; }
    }

    public class ContributorFilter : Filter
    {
        public int Id { get; set; }
    }

    public class IssueFilter: Filter
    {
        public int RepoId { get; set; }
        public string State { get; set; }
        public string Comment { get; set; }

    }


    public class Chart
    {
        public int RepoId { get; set; }
        public string Option { get; set; }
    }

    public class BranchChart : Chart
    {
        public int BranchId { get; set; }
    }

    public class ContributorChart : Chart
    {
        public int ContributorId { get; set; }
    }

}
