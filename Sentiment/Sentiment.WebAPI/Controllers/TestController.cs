using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sentiment.WebAPI.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public string get()
        {
            return "text";
        }
    }
}
