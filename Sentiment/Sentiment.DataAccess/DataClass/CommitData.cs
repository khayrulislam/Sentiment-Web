﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Commit")]
    public class CommitData
    {
        public int Id { get; set; }

        public ContributorData Commiter { get; set; }

        public BranchData Branch { get; set; }

        public int PosSentiment { get; set; }

        public int NegSentiment { get; set; }


    }
}