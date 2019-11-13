using Sentiment.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sentiment.WebAPI.Controllers
{
    public class DashboardController : ApiController
    {
        private DashboardService dashboard = new DashboardService();

        [HttpGet]
        public HttpResponseMessage GetCardData(int repoId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, dashboard.GetDashboardData(repoId));
        }
    }
}