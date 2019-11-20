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
        public int IssueNumber { get; set; }
        public int PosTitle { get; set; }
        public int NegTitle { get; set; }
        public string State { get; set; }
        public IssueType IssueType { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public ICollection<IssueCommentT> Comments { get; set; }

    }

    public class IssueView
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public int IssueNumber { get; set; }
        public int PosTitle { get; set; }
        public int NegTitle { get; set; }
        public string State { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public int CommentCount { get; set; }
    }
}
