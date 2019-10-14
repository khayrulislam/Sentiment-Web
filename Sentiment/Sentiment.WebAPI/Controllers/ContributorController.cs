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
    public class ContributorController : ApiController
    {
        private ContributorService contributorService = new ContributorService();

        [HttpGet]
        public List<ContributorT> GetContributorList(int repoId)
        {
            return contributorService.GetContributorList(repoId);
        }
    }
}
