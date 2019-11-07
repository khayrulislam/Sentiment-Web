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

    public enum AnalysisState
    {
        Runnig = 0,
        Complete
    }

    public enum IssueType
    {
        Issue=1,
        PullRequest
    }

    public class ExtensionInputData
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }

    public class ExtensionOutputData
    {
        public int PosSentiment { get; set; }
        public int NegSentiment { get; set; }
        public string Type { get; set; }
    }


}
