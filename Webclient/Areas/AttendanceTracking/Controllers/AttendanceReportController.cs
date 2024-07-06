using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;
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
    public class AttendanceReportController : BaseReportController
    {
        private readonly AttendanceReportBusiness _attendanceReportBusiness;
        public AttendanceReportController()
        {
            _attendanceReportBusiness = new AttendanceReportBusiness();
        }
        #region VeiwPage
        public ActionResult MonthlyAttendanceReports()
        {
            return PartialView();
        }
        public ActionResult MonthlyAttendanceReportDetails()
        {
            return PartialView();
        }
        public ActionResult AllUserAttendanceReports()
        {
            return PartialView();
        }
        #endregion
        [HttpGet]
        public JsonResult GetInitMonthYear()
        {
            var monthList = Enum.GetValues(typeof(MonthList)).Cast<MonthList>().Select(v => new NameIdPairModel
            {
                Id = (int)v,
                Name = EnumUtility.GetDescriptionFromEnumValue(v)
            }).ToList();
            var yearList = new List<int>();
            for (int i = 2020; i <= DateTime.UtcNow.Year; i++)
            {
                yearList.Add(i);
            }
            return Json(new { monthList, yearList, selectedMonth = DateTime.Now.Month, selectedYear = DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetMonthlyAttendanceReports(int monthId, int yearId)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var data = _attendanceReportBusiness.GetAttendanceReports(cId).Where(x=>x.AttendanceMonth==monthId && x.AttendanceYear==yearId).AsQueryable();
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllUserAttendanceReports(int monthId, int yearId)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var data = _attendanceReportBusiness.GetAllUserReports(cId).Where(x => x.AttendanceMonth == monthId && x.AttendanceYear == yearId).AsQueryable();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetMonthlyUserReports(GridSettings grid,string employeeId,int monthId,int yearId)
        {
            var query = _attendanceReportBusiness.GetUserReports(employeeId).Where(x => x.AttendanceMonth == monthId && x.AttendanceYear == yearId).AsQueryable();
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
        public void MonthlyAttendanceReportExportToExcel(int monthId, int yearId)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var listOfRequests = _attendanceReportBusiness.GetAttendanceReports(cId).Where(x=>x.AttendanceMonth==monthId && x.AttendanceYear==yearId).OrderBy(x => x.EmployeeCode);
            ExportToExcelAsFormated(ToReportExcelModel(listOfRequests.ToList()), "Monthly_Attendance_Report_Lists_" + DateTime.UtcNow + "_" + DateTime.UtcNow.Millisecond, null);
        }
        public void AllUserReportExportToExcel(int monthId, int yearId)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var data = _attendanceReportBusiness.GetAllUserReports(cId).Where(x => x.AttendanceMonth == monthId && x.AttendanceYear == yearId).AsQueryable();
            ExportToExcelAsFormated(ToEmployeeReportExcelModel(data.ToList()), "All_User_Attendance_Report_Lists_" + DateTime.UtcNow + "_" + DateTime.UtcNow.Millisecond, null);
        }
        private List<AttendanceReportExcelModel> ToReportExcelModel(List<AttendanceReportModel> list)
        {
            return (from item in list
                    select new AttendanceReportExcelModel
                    {
                        EmployeeCode=item.EmployeeCode,
                        EmployeeName = item.EmployeeName,
                        DepartmentName = item.DepartmentName,
                        DesignationName = item.DesignationName,
                        PresentDaysString=item.PresentDaysString,
                        AbsentDaysString=item.AbsentDaysString,
                        OfficeHours=item.OfficeHours,
                        CompletedHoursString=item.CompletedHoursString,
                        MissingOutTimeString=item.MissingOutTimeString,
                        OverTime=item.OverTimeString
                    }).ToList();
        }
        public void EmployeeMonthlyAttendanceReportExportToExcel(string employeeId, int monthId, int yearId)
        {
            var listOfRequests = _attendanceReportBusiness.GetUserReports(employeeId).Where(x => x.AttendanceMonth == monthId && x.AttendanceYear == yearId).OrderBy(x => x.EmployeeCode);
            ExportToExcelAsFormated(ToEmployeeReportExcelModel(listOfRequests.ToList()), "Employee_Monthly_AttendanceReport_Lists_" + DateTime.UtcNow + "_" + DateTime.UtcNow.Millisecond, null);
        }
        private List<EmployeeAttendanceReportExcelModel> ToEmployeeReportExcelModel(List<AttendanceReportModel> list)
        {
            return (from item in list
                    select new EmployeeAttendanceReportExcelModel
                    {
                        EmployeeName = item.EmployeeName,
                        DepartmentName = item.DepartmentName,
                        DesignationName = item.DesignationName,
                        LogInDate = item.LogInDate,
                        LogInTime = item.LogInTime,
                        LogInLocation = item.LogInLocation,
                        LogOutDate = item.LogInDate,
                        LogOutTime = item.LogOutTime,
                        LogOutLocation = item.LogOutLocation,
                        OfficeHours = item.OfficeHours,
                        CompletedHoursString = item.CompletedHoursString,
                        OverTime = item.OverTimeString,
                        DeviceName=item.DeviceName,
                        DeviceBrand=item.DeviceBrand,
                        DeviceModelName=item.DeviceModelName
                    }).ToList();
        }
    }
}
