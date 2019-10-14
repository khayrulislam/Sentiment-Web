using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.Shared
{
    public class Constant
    {
    }

    public enum CommentType{
        Commit = 0,
        Issue,
        PullRequest,
        Review
    }

    public class ExtensionData
    {
        public string Message { get; set; }
    }
}
