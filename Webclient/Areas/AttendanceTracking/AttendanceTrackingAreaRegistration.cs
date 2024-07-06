using System.Web.Mvc;

namespace Webclient.Areas.AttendanceTracking
{
    public class AttendanceTrackingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AttendanceTracking";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AttendanceTracking_default",
                "AttendanceTracking/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}