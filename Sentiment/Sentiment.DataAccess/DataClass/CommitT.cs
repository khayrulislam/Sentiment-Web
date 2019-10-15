﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Commit")]
    public class CommitT:Sentiment
    {
        public int Id { get; set; }
        public string Sha { get; set; }
        public int RepositoryId { get; set; }
        public IList<BranchCommitT> BranchCommits { get; set; }
    }
}
