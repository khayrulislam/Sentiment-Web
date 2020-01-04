using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class CommonService
    {

        public List<List<long>> GetSentimentLineChart(List<SentimentData> data)
        {
            List<List<long>> result = new List<List<long>>();
            data.ForEach(element => {
                if (element.Datetime != null)
                {
                    if (element.Pos > element.Neg * -1) result.Add(new List<long>() { element.Datetime.ToUnixTimeMilliseconds(), element.Pos });
                    //if (element.Pos == element.Neg * -1) result.Add(new List<long>() { element.Datetime.ToUnixTimeMilliseconds(), 0 }); // neutral value is 0
                    //else if (element.Pos > element.Neg * -1) result.Add(new List<long>() { element.Datetime.ToUnixTimeMilliseconds(), element.Pos });
                    else result.Add(new List<long>() { element.Datetime.ToUnixTimeMilliseconds(), element.Neg });
                }

            });

            return result;
        }

        public List<ChartData> GetSentimentPieChart(List<SentimentData> data)
        {
            List<ChartData> commitData = new List<ChartData>();
            int pos5 = 0, pos4 = 0, pos3 = 0, pos2 = 0, neg5 = 0, neg4 = 0, neg3 = 0, neg2 = 0, neutral = 0;
            data.ForEach(element => {

                if (element.Pos == 1 && element.Neg == -1) neutral++;
                //if (element.Pos == element.Neg * -1) neutral++;
                else if (element.Pos > element.Neg * -1)
                {
                    switch (element.Pos)
                    {
                        case 5:
                            pos5++;
                            break;
                        case 4:
                            pos4++;
                            break;
                        case 3:
                            pos3++;
                            break;
                        case 2:
                            pos2++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (element.Neg)
                    {
                        case -5:
                            neg5++;
                            break;
                        case -4:
                            neg4++;
                            break;
                        case -3:
                            neg3++;
                            break;
                        case -2:
                            neg2++;
                            break;
                        default:
                            break;
                    }
                }
            });

            commitData.Add(new ChartData()
            {
                name = "Negative[5]",
                value = neg5,
                extra = new ExtraCode() { code = "neg5" }
            });
            commitData.Add(new ChartData()
            {
                name = "Negative[4]",
                value = neg4,
                extra = new ExtraCode() { code = "neg4" }
            });
            commitData.Add(new ChartData()
            {
                name = "Negative[3]",
                value = neg3,
                extra = new ExtraCode() { code = "neg3" }
            });
            commitData.Add(new ChartData()
            {
                name = "Negative[2]",
                value = neg2,
                extra = new ExtraCode() { code = "neg2" }
            });
            commitData.Add(new ChartData()
            {
                name = "Neutral",
                value = neutral,
                extra = new ExtraCode() { code = "neutral" }
            });
            commitData.Add(new ChartData()
            {
                name = "Positive[2]",
                value = pos2,
                extra = new ExtraCode() { code = "pos2" }
            });
            commitData.Add(new ChartData()
            {
                name = "Positive[3]",
                value = pos3,
                extra = new ExtraCode() { code = "pos3" }
            });
            commitData.Add(new ChartData()
            {
                name = "Positive[4]",
                value = pos4,
                extra = new ExtraCode() { code = "pos4" }
            });
            commitData.Add(new ChartData()
            {
                name = "Positive[5]",
                value = pos5,
                extra = new ExtraCode() { code = "pos5" }
            });

            return commitData;
        }


        public string RemoveGitHubTag(string text)
        {

            text = Regex.Replace(text, @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})","");
            text = Regex.Replace(text, @"#+\s|==+|--+|\*+|_+|~+|·+|`.*`|```.*```|<.*>|!\s*\[.*\]+", "");
            text = Regex.Replace(text, @"</.*>", " .");
            text = Regex.Replace(text, @"<|>", "");


            /*
                        string header = Regex.Replace(str, @"#+\s|==+|--+", "");

                        string bold = Regex.Replace(str, @"\*+|_+|~+", "");

                        string list = Regex.Replace(str, @"·+", "");


                        string inlinecode = Regex.Replace(str, @"`.*`", "");

                        string code = Regex.Replace(str, @"```.*```", "");

                        string starttag = Regex.Replace(str, @"<.*>", "");

                        string endtag = Regex.Replace(str, @"</.*>", " .");

                        @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})"



                        string image = Regex.Replace(str, @"!\s*\[.*\]+", "");*/

            return text;
        }

    }
}
