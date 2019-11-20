using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.Shared;
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
    public class ContributorController : ApiController
    {
        private ContributorService contributorService = new ContributorService();

        [HttpGet]
        public HttpResponseMessage GetContributorList(int repoId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, contributorService.GetContributorList(repoId));
        }

        [HttpPost]
        public HttpResponseMessage GetFilterList(ContributorFilter filter)
        {
            return Request.CreateResponse(HttpStatusCode.OK, contributorService.GetFilterList(filter));
        }

        [HttpPost]
        public HttpResponseMessage GetDetail(ContributorChart contributorChart)
        {
            return Request.CreateResponse(HttpStatusCode.OK, contributorService.GetDetail(contributorChart));
        }

        [HttpPost]
        public HttpResponseMessage GetCommitChartData(ContributorChart contributorChart)
        {
            return Request.CreateResponse(HttpStatusCode.OK, contributorService.GetCommitDetail(contributorChart));
        }

        [HttpPost]
        public HttpResponseMessage GetIssueChartData(ContributorChart contributorChart)
        {
            return Request.CreateResponse(HttpStatusCode.OK, contributorService.GetIssueDetail(contributorChart));
        }

        [HttpPost]
        public HttpResponseMessage GetpullRequestChartData(ContributorChart contributorChart)
        {
            return Request.CreateResponse(HttpStatusCode.OK, contributorService.GetPullRequestDetail(contributorChart));
        }
    }
}
