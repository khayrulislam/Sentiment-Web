﻿using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("IssueComment")]
    public class IssueCommentT:SentimentComment
    {
        public int Id { get; set; }
        public long CommentNumber { get; set; }
        public int IssueId { get; set; }
        public IssueT Issue { get; set; }
        public DateTimeOffset DateTime { get; set; }

    }
}