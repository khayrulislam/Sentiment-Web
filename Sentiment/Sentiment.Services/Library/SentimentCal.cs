using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.ac.wlv.sentistrength;

namespace Sentiment.Services.Library
{
    public sealed class SentimentCal
    {
        public static SentimentCal sentiInstance = null;
        private static readonly object padlock = new object();
        private SentiStrength senti;

        public int PositoiveSentiScore { get; private set; }
        public int NegativeSentiScore { get; private set; }

        private SentimentCal()
        {
            senti = new SentiStrength();
            senti.initialise(new string [] { "inputfolder", "../../../Sentiment.Services/Library/Data" });
        }

        public static SentimentCal Instance
        {
            get
            {
                if (sentiInstance == null)
                {
                    lock (padlock)
                    {
                        if (sentiInstance == null)
                        {
                            sentiInstance = new SentimentCal();
                        }
                    }
                }
                return sentiInstance;
            }
        }

        public void CalculateSentiment(string sentence)
        {
            string value = senti.computeSentimentScores(sentence);
            string[] values = value.Split(' ');
            this.PositoiveSentiScore = Convert.ToInt32(values[0]);
            this.NegativeSentiScore = Convert.ToInt32(values[1]);
        }
    }
}
