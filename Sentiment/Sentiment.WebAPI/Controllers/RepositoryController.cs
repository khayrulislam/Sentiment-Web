using Sentiment.DataAccess.Shared;
using Sentiment.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sentiment.WebAPI.Controllers
{
    public class RepositoryController : ApiController
    {
        private RepositoryService repositoryService = new RepositoryService();
        
        [HttpGet]
        public HttpResponseMessage ExecuteAnalysis(string repoOwnerName, string repoName)
        {
            Task.Run(() =>
            {
                repositoryService.ExecuteAnalysisAsync(repoName, repoOwnerName);
            });
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage GetList()
        {
            return Request.CreateResponse(HttpStatusCode.OK, repositoryService.GetList());
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, repositoryService.Get(id));
        }

        [HttpGet]
        public HttpResponseMessage GetById(long repoId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, repositoryService.GetById(repoId));
        }

        [HttpPost]
        public HttpResponseMessage GetListByFilter(RepositroyFilter filter)
        {
            return Request.CreateResponse(HttpStatusCode.OK, repositoryService.GetFilterList(filter));
        }
    }
}
