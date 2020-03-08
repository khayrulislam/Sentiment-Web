using Sentiment.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Sentiment.WebAPI.Controllers
{
    public class DownloadController : ApiController
    {
        private DownloadService downloadService = new DownloadService();
        private RepositoryService repositoryService = new RepositoryService();

        [HttpGet]
        public HttpResponseMessage DownloadRepository(int repoId)
        {
            var data = downloadService.GetRepositoryContent(repoId);

            var repository = repositoryService.Get(repoId);

            var response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new ByteArrayContent(data) ;
            response.Content.Headers.ContentLength = data.Length;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = repository.Name + ".xlsx";

            return response;
        }

    }
}
