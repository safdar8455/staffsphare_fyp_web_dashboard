using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Webclient.Models;

namespace Webclient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserCredential _userCredential;
        private readonly ICompany _companyRepository;
        public AccountController()
        {
            _userCredential = AttendanceUnityMapper.GetInstance<IUserCredential>();
            _companyRepository = AttendanceUnityMapper.GetInstance<ICompany>();
        }

        [HttpGet]
        public ActionResult Login()
        {

            return View(new LoginModel());
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetUser(model);
                if(user==null)
                {
                    ViewData["ErrorTxt"] = "Login was unsuccessful.";
                    return View(model);
                }


                FormsAuthentication.SetAuthCookie(model.LoginID, model.RememberMe);
                var companyLogo = string.Empty;
                if (user.UserTypeId != (int)UserType.SuperAdmin)
                {
                    var companyDetails = _companyRepository.GetCompanyList().FirstOrDefault(x => x.Id == user.CompanyId && x.IsActive.HasValue && x.IsActive.Value);
                    if (companyDetails != null)
                        companyLogo = companyDetails.ImageFileId;

                }

                System.Web.HttpContext.Current.Session[Constants.CurrentUser] = new UserSessionModel
                {
                    Id = user.Id,
                    UserTypeId = user.UserTypeId,
                    FullName = user.FullName,
                    UserInitial = model.LoginID,
                    Email = user.Email,
                    CompanyId = user.CompanyId,
                    CompanyLogo = companyLogo
                };

                var redirectView = "CompanySettings";
                switch (user.UserTypeId)
                {
                    case (int)UserType.SuperAdmin:
                        redirectView= "CompanySettings";
                        break;
                    case (int)UserType.Admin:
                        redirectView = "Dashboard";
                        break;
                    case (int)UserType.Employee:
                        redirectView = "EmployeeDashboard";
                        break;
                }
                return RedirectToAction("Index", redirectView, new { Area = user.UserTypeId==(int)UserType.Employee? "EmployeeRole": "AttendanceTracking" });
            }

            ViewData["ErrorTxt"] = "Login was unsuccessful.";
            return View(model);
        }

        private UserCredentialModel GetUser(LoginModel objUser)
        {
            var password = CryptographyHelper.CreateMD5Hash(objUser.Password);
            var user = _userCredential.Get(objUser.LoginID, password);
            return user;
        }

        public ActionResult LogOff()
        {
            return RedirectToLogOff();
        }

        public ActionResult RedirectToLogOff()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
