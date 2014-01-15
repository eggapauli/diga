using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Diga.Cloud.WebStatus
{
    public class StatusController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Hello from OWIN!");
        }

        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, string.Format("Hello from OWIN! (id = {0})", id));
        }
    }
}
