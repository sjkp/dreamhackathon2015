using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackendWeb.Controllers
{
    public class ManageController : ApiController
    {
        [Route("api/manage")]
        [HttpGet]
        public IEnumerable<string> Gets()
        {
            return new string[] { "test" };
        }

        [Route("api/manage")]
        [HttpPost]
        public IHttpActionResult Post()
        {
            Scoreboard.Instance.Users = new Dictionary<string, Score>();
            Scoreboard.Instance.UserBeats = new Dictionary<string, List<int>>();
            return Ok();
        }
    }
}
