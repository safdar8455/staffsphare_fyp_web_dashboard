using Ems.BusinessTracker.Common;
using System;
using System.Linq;
using System.Web.Mvc;
using Ems.AttendanceTracking.Models;
using Ems.AttendanceTracking.Services;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class LeaveController : BaseController
    {
        private readonly LeaveBusiness _leaveBusiness;
        public LeaveController()
        {
            _leaveBusiness = new LeaveBusiness();
        }
        #region ViewPage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyPendingLeaves()
        {
            return View();
        }
        public ActionResult AnnualLeaveCalculation()
        {
            return View();
        }
        public ActionResult LeaveDetails()
        {
            return View();
        }
        public ActionResult RejectReason()
        {
            return View();
        }
        #endregion

        [HttpGet]
        public JsonResult GetCompanyPendingLeaves(GridSettings grid)
        {
            int cId = 0;
            if (_userInfo.UserTypeId == (int)UserType.SuperAdmin)
                cId = 0;
            else
                cId = _userInfo.CompanyId ?? 0;
            var query = _leaveBusiness.GetLeaveByCompanyId(cId).Where(x=>x.StatusId==(int)LeaveStatus.Pending).AsQueryable();
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
        public JsonResult GetAnnualLeaveCalculation(GridSettings grid)
        {
            // to do not completed
            var query = _leaveBusiness.GetUserPendingLeaves(this._userInfo.Id).AsQueryable();
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
        public JsonResult GetCompanyLeaves(GridSettings grid)
        {
            int cId = 0;
            if (_userInfo.UserTypeId == (int)UserType.SuperAdmin)
                cId = 0;
            else
                cId = _userInfo.CompanyId??0;
            var query = _leaveBusiness.GetLeaveByCompanyId(cId).AsQueryable();
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
        public JsonResult Get(int id)
        {
            return Json(_leaveBusiness.GetLeaveById(id, _userInfo.Id), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ApproveOrRejectLeave(LeaveModel model)
        {
            var leaveModel = new LeaveApproveModel
            {
                RequestNo = model.RequestNo,
                SerialNo = model.ApproverSerialId,
                Approved = model.IsApproved,
                Rejected = model.IsRejected,
                RejectReason = model.RejectReason,
                ApproverId = _userInfo.Id,
                UpdatedById = _userInfo.Id,
                UpdatedAt = DateTime.UtcNow
            };
            var response = _leaveBusiness.ApproveOrRejectLeave(leaveModel);
            return Json(response);
        }
    }
}
