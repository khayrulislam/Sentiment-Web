using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("PullCommit")]
    public class PullCommitT
    {
        public int Id { get; set; }
        //public int PullNumber { get; set; }
        public string CommitSha { get; set; }
        public IssueT PullRequest { get; set; }
        public int PullRequestId { get; set; }

    }
}
