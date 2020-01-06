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
        Review,
        
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


    public class project
    {
        public string Name { get; set; }

    }

    public enum RepositorySheets
    {
        //Branch,
        //Commit,
        Issue,
        Issue_Comment,
        Pull_Request,
        //Commit_Comment,
        Pull_Request_Comment,
        Review_Comment
    }

    public enum BranchHeader
    {
        Id,
        Name,
        Sha
    }

    public enum CommitHeader
    {
        Id,
        Sha,
        Pos_Sentiment,
        Neg_Sentiment,
        DateTime,
        Message
    }

    public enum IssueHeader
    {
        //Id,
        IssueNumber,
        //Status,
        Title_Pos_Sentiment,
        Title_Neg_Sentiment,
        Title,
        Body_Pos_Sentiment,
        Body_Neg_Sentiment,
        Body,
        Create_Date,
        Close_Date,
        Labels, 
        Participants, // assignees
        Creator,
        Comments
    }

    public enum PullHeader
    {
        //Id,
        PullRequestNumber,
        //Status,
        Title_Pos_Sentiment,
        Title_Neg_Sentiment,
        Title,
        Body_Pos_Sentiment,
        Body_Neg_Sentiment,
        Body,
        Create_Date,
        Close_Date,
        Merge_date,
        Labels,
        Participants, // assignees
        Creator,
        Comments,
        Merged,
        Reviews,
        //Commits
    }

    public enum CommitCommentHeader
    {
        Id,
        CommitId,
        CommentNumber,
        Date,
        Pos_Sentiment,
        Neg_Sentiment,
        Message
    }

    public enum IssueCommentHeader
    {
        //Id,
        IssueId,
        CommentNumber,
        Date,
        Pos_Sentiment,
        Neg_Sentiment,
        Message,
        Creator
    }

    public enum PullCommentHeader
    {
        //Id,
        PullRequestId,
        CommentNumber,
        Date,
        Pos_Sentiment,
        Neg_Sentiment,
        Message,
        Creator
    }

}
