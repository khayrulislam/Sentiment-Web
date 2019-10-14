using Sentiment.DataAccess.Shared;
using Sentiment.Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sentiment.WebAPI.Controllers
{
    public class SentimentController : ApiController
    {
        SentimentCal sentimentCal = SentimentCal.Instance;

        [HttpPost]
        public List<int> CalculateSentiment(ExtensionData data)
        {
            sentimentCal.CalculateSentiment(data.Message);
            return new List<int>() { sentimentCal.PositoiveSentiScore,sentimentCal.NegativeSentiScore};
        }
    }
}
