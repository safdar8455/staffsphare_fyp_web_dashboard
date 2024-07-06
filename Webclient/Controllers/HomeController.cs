using System.Web.Mvc;
using Webclient.Filters;

namespace Webclient.Controllers
{
    [SessionHelper]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegistrationSuccess(string email)
        {

            ViewBag.Email = email;
            return View();
        }
        public ActionResult UpdateProfile()
        {
            return PartialView();
        }
    }
}
