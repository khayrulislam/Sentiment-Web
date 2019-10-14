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
        public  string ExecuteRepositoryAnalysis(int userId, string repoName, string repoOwnerName)
        {
            Task.Run(() =>
            {
                repositoryService.ExecuteAnalysisAsync(userId, repoName, repoOwnerName);
            });

            //await repositoryService.ExecuteAnalysisAsync(userId,repoName,repoOwnerName).ConfigureAwait(false);
            return "yuyuyuy";
        }
    }
}
