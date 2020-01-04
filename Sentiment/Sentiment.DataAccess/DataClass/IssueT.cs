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
        public DateTimeOffset? CreateDate{ get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public DateTimeOffset? CloseDate { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public ICollection<IssueCommentT> Comments { get; set; }
        public string Lables { get; set; } // comma seperated lable name
        public string Assignees { get; set; } // comma seperated assignees login

    }

    public class IssueView
    {
        public int Id { get; set; }
        public int IssueNumber { get; set; }
        public int Pos { get; set; }
        public int Neg { get; set; }
        public int PosTitle { get; set; }
        public int NegTitle { get; set; }
        public string State { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public int CommentCount { get; set; }
    }
}
