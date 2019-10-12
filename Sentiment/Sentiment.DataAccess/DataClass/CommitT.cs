using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Commit")]
    public class CommitT
    {
        public int Id { get; set; }
        public int CommiterId { get; set; }
        public ContributorT Commiter { get; set; }
        public string Message { get; set; }
        public string Sha { get; set; }
        public int PosSentiment { get; set; }
        public int NegSentiment { get; set; }
        public IList<BranchCommitT> BranchCommits { get; set; }
    }
}
