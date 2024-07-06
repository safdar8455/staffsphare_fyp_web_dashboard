using Ems.BusinessTracker.Common;
using System.Web.Mvc;
using Webclient.Controllers;
using Webclient.Filters;
using Ems.BusinessTracker.Common.Models;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Webclient.Areas.EmployeeRole.Controllers
{
    [SessionHelper]
    public class ChangePasswordController : BaseController
    {
        private readonly IUserCredential _userCredential;

        public ChangePasswordController()
        {
            _userCredential = AttendanceUnityMapper.GetInstance<IUserCredential>();
        }

        public ActionResult ChangePassword()
        {
            return PartialView();
        }
        [HttpGet]
        public JsonResult GetInitUser()
        {
            var passwordModel = new PasswordModel
            {
                UserName = _userInfo.UserInitial
            };
            return Json(passwordModel, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdatePassword(PasswordModel model)
        {
            var response = new ResponseModel();
            var userDetails = _userCredential.GetProfileDetails(_userInfo.Id);
            model.NewPassword = CryptographyHelper.CreateMD5Hash(model.NewPassword);
            model.OldPassword = CryptographyHelper.CreateMD5Hash(model.OldPassword);
            if (userDetails.OldPassword != model.OldPassword)
            {
                response = new ResponseModel { Success = false, Message = "Old Password Does not match" };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            if (model.NewPassword == model.OldPassword)
            {
                response = new ResponseModel { Success = false, Message = "New Password can't be same as Old Password" };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            response = _userCredential.ChangePassword(model.UserName, model.NewPassword);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
