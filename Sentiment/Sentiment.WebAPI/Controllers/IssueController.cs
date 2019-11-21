using Sentiment.DataAccess.Shared;
using Sentiment.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sentiment.WebAPI.Controllers
{
    public class IssueController : ApiController
    {
        private IssueService issueService = new IssueService();

        [HttpPost]
        public HttpResponseMessage GetChartData(Chart chartParam)
        {
            return Request.CreateResponse(HttpStatusCode.OK, issueService.GetChartData(chartParam.RepoId, chartParam.Option));
        }

        [HttpPost]
        public HttpResponseMessage GetPullRequestChartData(Chart chartParam)
        {
            return Request.CreateResponse(HttpStatusCode.OK, issueService.GetPullRequestChartData(chartParam.RepoId, chartParam.Option));
        }

        [HttpPost]
        public HttpResponseMessage GetFilterList(IssueFilter filter)
        {
            return Request.CreateResponse(HttpStatusCode.OK, issueService.GetFilterList(filter));
        }

        [HttpPost]
        public HttpResponseMessage GetPullRequestFilterList(IssueFilter filter)
        {
            return Request.CreateResponse(HttpStatusCode.OK, issueService.GetPullRequestFilterList(filter));
        }


        [HttpPost]
        public HttpResponseMessage GetFilterSentimentData(IssueFilter filter)
        {
            return Request.CreateResponse(HttpStatusCode.OK, issueService.GetFilterChartData(filter));
        }

        [HttpPost]
        public HttpResponseMessage GetPullRequestFilterSentimentData(IssueFilter filter)
        {
            return Request.CreateResponse(HttpStatusCode.OK, issueService.GetPullRequestFilterChartData(filter));
        }


    }
}
