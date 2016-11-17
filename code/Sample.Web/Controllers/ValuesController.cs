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
            // NB Not sure why User is null - thoughy default principal would be assigned.
            if (User == null || User.Identity.IsAuthenticated == false)
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
        public int Post(string value)
        {
            if (User == null || User.Identity.IsAuthenticated == false)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            return 1;
        }
    }
}