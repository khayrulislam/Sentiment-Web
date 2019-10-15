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
    public class BranchController : ApiController
    {
        private BranchService branchService = new BranchService();

        [ResponseType(typeof(List<BranchT>))]
        [HttpGet]
        public HttpResponseMessage GetBranchList(int repoId)
        {
            return  Request.CreateResponse(HttpStatusCode.OK, branchService.GetBranchList(repoId));
        }

    }
}
