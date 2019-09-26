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
        private RepositoryService repo = new RepositoryService();
        
        [HttpGet]
        public async Task CalculateRepositorySentiment()
        {
            string repositoryUrl = null;
            // asyn method call
            await repo.ExecuteRepositoryAnalysisAsync(repositoryUrl);
        }
    }
}
