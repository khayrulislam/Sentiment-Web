using com.sun.security.ntlm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var buildDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var path = Path.Combine(buildDir,"bin\\Data\\");
            senti = new SentiStrength();
            senti.initialise(new string [] { "sentidata",path});
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
            this.NegativeSentiScore = 0;
            this.PositoiveSentiScore = 0;
            string value = senti.computeSentimentScores(sentence);
            string[] values = value.Split(' ');
            this.PositoiveSentiScore = Convert.ToInt32(values[0]);
            this.NegativeSentiScore = Convert.ToInt32(values[1]);
        }
    }
}
