using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;
using Ems.AttendanceTracking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class DashboardController : BaseReportController
    {
        private readonly IAttendance _attendance;
        private readonly IEmployee _Employee;
        public DashboardController()
        {
            _attendance = AttendanceUnityMapper.GetInstance<IAttendance>();
            _Employee = AttendanceUnityMapper.GetInstance<IEmployee>();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExpiryDetails()
        {
            return PartialView();
        }
        [HttpGet]
        public JsonResult GetExpiryReports(GridSettings grid,int expiryId)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var query = _attendance.GetExpiryReports(expiryId,cId).AsQueryable();
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
        public void ExpiryReportExportToExcel(int expiryId)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var query = _attendance.GetExpiryReports(expiryId, cId).AsQueryable();
            ExportToExcelAsFormated(ToExpiryExcelModel(query.ToList()), "Expiry_Report_Lists_" + DateTime.UtcNow + "_" + DateTime.UtcNow.Millisecond, null);
        }
        private List<EmployeeDocExpiryReportModel> ToExpiryExcelModel(List<EmployeeDetailsModel> list)
        {
            return (from item in list
                    select new EmployeeDocExpiryReportModel
                    {
                        EmployeeCode = item.EmployeeCode,
                        EmployeeName = item.EmployeeName,
                        MobileNo = item.MobileNo,
                        PassportNo = item.PassportNo,
                        PassportExpiryDateVw = item.PassportExpiryDateVw,
                        QID = item.QID,
                        QIDExpiryDateVw = item.QIDExpiryDateVw,
                        VisaNo = item.VisaNo,
                        VisaExpirayDate = item.VisaExpiryDateVw,
                        HealthCardNo = item.HealthCardNo,
                        HealthCardExpiryVw = item.HealthCardExpiryVw
                    }).ToList();
        }

        [HttpGet]
        public JsonResult GetLeaveStatus()
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var query = _attendance.GetLeaveStatusList(DateTime.UtcNow, cId).AsQueryable();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDocExpiryCount()
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var data = _Employee.GetEmployeeDocExpiry(cId);

            var listOfData = new List<EmployeeDocExpiryModel>
            {
                new EmployeeDocExpiryModel { Id = 1, Name = "Expired", Qid = data.QidExpiry, Visa = data.VisaExpiry, Passport = data.PassportExpiry, HealthCard = data.HealthExpiry },
                new EmployeeDocExpiryModel { Id = 2, Name = "Expiring in 90 days", Qid = data.QidExpiry90Days, Visa = data.VisaExpiry90Days, Passport = data.PassportExpiry90Days, HealthCard = data.HealthExpiry90Days },
                new EmployeeDocExpiryModel { Id = 3, Name = "Expiring in 60 days", Qid = data.QidExpiry60Days, Visa = data.VisaExpiry60Days, Passport = data.PassportExpiry60Days, HealthCard = data.HealthExpiry60Days },
                new EmployeeDocExpiryModel { Id = 4, Name = "Expiring in 30 days", Qid = data.QidExpiry30Days, Visa = data.VisaExpiry30Days, Passport = data.PassportExpiry30Days, HealthCard = data.HealthExpiry30Days }
            };

            return Json(listOfData, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet]
        public JsonResult GetNoticeBoard()
        {
            var query = new NoticeBoardBusiness().GetAll(_userInfo.CompanyId ?? 0).AsQueryable();
            int i = 0;
            foreach (var item in query)
            {
                item.NoticeId = ++i;
            }
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAttendanceFeed()
        {
            var result = _attendance.GetAttendanceFeed(DateTime.UtcNow, _userInfo.CompanyId ?? 0);
            return Json(new
            {
                EmployeeList = result,
                StatusCount = new
                {
                    TotalEmployee = result.Count,
                    TotalCheckIn = result.Count(x => x.CheckInTime.HasValue),
                    TotalCheckOut = result.Count(x => x.CheckOutTime.HasValue),
                    TotalNotAttend = result.Count(x => !x.CheckInTime.HasValue)
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAttendanceFeedGrid(GridSettings grid)
        {

            var query = _attendance.GetAttendanceFeed(DateTime.UtcNow, _userInfo.CompanyId ?? 0).AsQueryable();
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

    }
}
