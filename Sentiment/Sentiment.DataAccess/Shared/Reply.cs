using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.Shared
{
    public class ReplyList<TEntity>
    {
        public ReplyList()
        {
            Data = new List<TEntity>();
        }
        public int TotalData { get; set; }
        public List<TEntity> Data { get; set; }
    }

    public class ExtraCode
    {
        public string code { get; set; }
    }

    public class ChartData
    {
        public string name { get; set; }
        public int value { get; set; }
        public ExtraCode extra { get; set; }
    }

    public class SentimentData
    {
        public int Pos { get; set; }
        public int Neg { get; set; }
        public DateTimeOffset Datetime { get; set; }
    }


    public class ReplyChart
    {
        public List<List<long>> LineData { get; set; }
        public List<ChartData> PieData { get; set; }
    }

    public class ReplyContributorDetail
    {

        public ReplyChart Commit { get; set; }
        public ReplyChart Issue { get; set; }
        public ReplyChart PullRequest { get; set; }

        public ReplyContributorDetail()
        {
            Commit = new ReplyChart();
            Issue = new ReplyChart();
            PullRequest = new ReplyChart();
        }
    }

}
