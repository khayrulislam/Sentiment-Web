using Sentiment.DataAccess.DataClass;
using Sentiment.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sentiment.WebAPI.Controllers
{
    public class BranchController : ApiController
    {
        private BranchService branchService = new BranchService();

        [HttpGet]
        public List<BranchT> GetBranchList(int repoId)
        {
            return branchService.GetBranchList(repoId);
        }

    }
}
