using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Webclient.Helpers
{
    public static class ScriptHelper
    {
        public static HtmlString Script(this UrlHelper helper, string contentPath)
         {
             return new HtmlString (string.Format("<script type='text/javascript' src='{0}'></script>" , LatestContent(helper, contentPath)));
         }

        public static HtmlString Css(this UrlHelper helper, string contentPath)
        {
            return new HtmlString(string.Format("<link rel='stylesheet' href='{0}' />", LatestContent(helper, contentPath)));
        }

         public static string LatestContent(this UrlHelper helper, string contentPath)
         {
             string file = HttpContext.Current.Server.MapPath(contentPath);
             if  (File.Exists(file))
             {
                 var dateTime = File.GetLastWriteTime(file);
                contentPath = string.Format("{0}?v={1}", contentPath, dateTime.Ticks);
           }

            return helper.Content(contentPath);
         }
    }
}