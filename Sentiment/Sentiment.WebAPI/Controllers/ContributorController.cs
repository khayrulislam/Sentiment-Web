using Sentiment.DataAccess.DataClass;
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

        [ResponseType(typeof(List<ContributorT>))]
        [HttpGet]
        public HttpResponseMessage GetContributorList(int repoId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, contributorService.GetContributorList(repoId));
        }
    }
}
