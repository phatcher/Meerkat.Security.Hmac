using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Sample.Web.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Secure()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return Unauthorized();
            }
            return Ok(new [] { "A", "B" });
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Insecure()
        {
            return Ok(new[] { "C", "D" });
        }

        [HttpPost]
        public IHttpActionResult Post(string value)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return Unauthorized();
            }

            return Ok(1);
        }
    }
}