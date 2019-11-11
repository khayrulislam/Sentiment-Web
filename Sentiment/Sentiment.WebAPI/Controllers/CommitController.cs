using Sentiment.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sentiment.WebAPI.Controllers
{
    public class CommitController : ApiController
    {
        private CommitService commitService = new CommitService();

        [HttpGet]
        public HttpResponseMessage GetOnlySentimentCommitData(int repoId, string option)
        {
            return Request.CreateResponse(HttpStatusCode.OK,commitService.GetChartData(repoId,option));
        }

    }
}
