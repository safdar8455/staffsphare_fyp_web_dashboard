using System.Web.Http;

namespace Webclient.Controllers.Api
{
    [JwtAuthentication]
    public class BaseApiController : ApiController
    {

        public string UserId => this.User.Identity.GetUserId();

        public int CompanyId => this.User.Identity.GetCompanyId();

    }
}
