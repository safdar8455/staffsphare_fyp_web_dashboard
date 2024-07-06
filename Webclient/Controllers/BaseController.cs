using Ems.BusinessTracker.Common.Models;
using System.Web.Mvc;
using Webclient.Filters;

namespace Webclient.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserSessionModel _userInfo;
        public BaseController()
        {
            _userInfo = SessionHelper.GetCurrentUser();
        }

    }
}