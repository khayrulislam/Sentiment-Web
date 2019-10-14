using Sentiment.DataAccess.Shared;
using Sentiment.Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Sentiment.WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SentimentController : ApiController
    {
        SentimentCal sentimentCal = SentimentCal.Instance;
        [AllowAnonymous]
        [HttpPost]
        public ExtensionOutputData CalculateSentiment(ExtensionInputData data)
        {
            sentimentCal.CalculateSentiment(data.Message);
            //Response.AppendHeader("Access-Control-Allow-Origin", "*");
            ExtensionOutputData od = new ExtensionOutputData()
            {
                NegSentiment = sentimentCal.NegativeSentiScore,
                PosSentiment = sentimentCal.PositoiveSentiScore,
                Type =data.Type
            };
            return od;
        }
    }
}
