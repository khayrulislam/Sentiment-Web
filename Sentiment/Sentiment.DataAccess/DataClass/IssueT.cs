using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Issue")]
    public class IssueT:Sentiment
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public RepositoryT Repository { get; set; }
        public long IssueNumber { get; set; }
        public int PosSentimentTitle { get; set; }
        public int NegSentimentTitle { get; set; }
        public string State { get; set; }
        public IssueType IssueType { get; set; }
        public DateTimeOffset? DateTime { get; set; }
        public ICollection<IssueCommentT> Comments { get; set; }

    }
}
