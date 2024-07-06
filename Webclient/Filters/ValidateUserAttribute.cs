using System.Web;
using System.Web.Mvc;
using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;

namespace Webclient.Filters
{

    public class SessionHelper : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // check  sessions here
            if (GetCurrentUser() == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public static UserSessionModel GetCurrentUser()
        {
            return HttpContext.Current.Session[Constants.CurrentUser] as UserSessionModel;
        }

    }


}