using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;

namespace Webclient.Helpers
{
    public static class CommonUtility
    {
        static CommonUtility()
        {
            
        }
        public static UserSessionModel GetCurrentUser()
        {
            return System.Web.HttpContext.Current.Session[Constants.CurrentUser] as UserSessionModel;
        }

        public static string UserName
        {
            get
            {
                var loggedInUser = GetCurrentUser();
                return loggedInUser!=null? loggedInUser.FullName:string.Empty;
            }
        }
        public static bool IsSuperAdmin
        {
            get
            {
                var loggedInUser = GetCurrentUser();
                return loggedInUser != null && (UserType)loggedInUser.UserTypeId == UserType.SuperAdmin;
            }
        }
        public static bool IsAdmin
        {
            get
            {
                var loggedInUser=GetCurrentUser();
                return loggedInUser != null && (UserType)loggedInUser.UserTypeId==UserType.Admin;
            }
        }

        public static bool IsEmployee
        {
            get
            {
                var loggedInUser = GetCurrentUser();
                return loggedInUser != null && (UserType)loggedInUser.UserTypeId == UserType.Employee;
            }
        }

    }
}