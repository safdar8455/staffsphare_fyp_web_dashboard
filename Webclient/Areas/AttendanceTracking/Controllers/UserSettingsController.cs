using Ems.BusinessTracker.Common.Models;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class UserSettingsController : BaseController
    {
        private readonly IUserCredential _userCredential;
        public UserSettingsController()
        {
            _userCredential = AttendanceUnityMapper.GetInstance<IUserCredential>();
        }
        public ActionResult UserList()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetUserList(GridSettings grid)
        {
            var query = _userCredential.GetAllUser().AsQueryable();
            if (_userInfo.CompanyId.HasValue && _userInfo.CompanyId > 0)
                query = query.Where(x => x.CompanyId == _userInfo.CompanyId);

            var listOfFilteredData = FilterHelper.JQGridFilter(query, grid).AsQueryable();
            var listOfPagedData = FilterHelper.JQGridPageData(listOfFilteredData, grid);
            var count = listOfFilteredData.Count();
            var result = new
            {
                total = (int)Math.Ceiling((double)count / grid.PageSize),
                page = grid.PageIndex,
                records = count,
                rows = listOfPagedData
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUserDetails(string id)
        {
            var result = _userCredential.GetProfileDetails(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(UserCredentialModel model)
        {
            var result = _userCredential.Save(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(UserCredentialModel model)
        {
            var result = _userCredential.Update(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginHistory()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetUserLoginHistoryList(GridSettings grid)
        {
            var query = _userCredential.GetAllUser(this._userInfo.CompanyId??0).AsQueryable();

            var listOfFilteredData = FilterHelper.JQGridFilter(query, grid).AsQueryable();
            var listOfPagedData = FilterHelper.JQGridPageData(listOfFilteredData, grid);
            var count = listOfFilteredData.Count();
            var result = new
            {
                total = (int)Math.Ceiling((double)count / grid.PageSize),
                page = grid.PageIndex,
                records = count,
                rows = listOfPagedData
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RemoveDevice(string uId)
        {
            var result = _userCredential.RemoveDevice(uId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ResetUserPass(string uId)
        {
            var result = _userCredential.ResetUserPass(uId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
