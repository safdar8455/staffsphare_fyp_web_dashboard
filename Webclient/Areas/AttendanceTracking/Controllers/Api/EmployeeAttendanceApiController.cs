using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;
using Webclient.Controllers.Api;


namespace Webclient.Areas.AttendanceTracking.Controllers.Api
{
    public class EmployeeAttendanceApiController : BaseApiController
    {
        private readonly IAttendance _attendance;
        private readonly IEmployee _Employee;
        public EmployeeAttendanceApiController()
        {
            _attendance = AttendanceUnityMapper.GetInstance<IAttendance>();
            _Employee = AttendanceUnityMapper.GetInstance<IEmployee>();

        }
        /// <summary>
        /// while checkin that time system calculate also office hour from company setup maximum office hour
        /// Daily only one checkin possible.so if anyone checkout after checkin then that employee not possible to checkin again for that day
        /// On checkin time system automaticaly get user Latitude/Logitude using gps. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CheckIn(AttendanceEntryModel model)
        {
            var attendanceFeed = _attendance.GetAttendanceFeed(DateTime.UtcNow, this.CompanyId > 0 ? this.CompanyId : (int?)null).ToList();
            if (attendanceFeed.Any(x => x.UserId == model.UserId && x.IsCheckedIn))
                return Ok(new ResponseModel {Message="Already checked-in." });

            model.CompanyId = this.CompanyId;
            var response = _attendance.CheckIn(model);
            if (response.Success)
            {
                _attendance.SaveCheckPoint(new UserMovementLogModel {
                    UserId=model.UserId,
                    Latitude=model.Latitude,
                    Longitude=model.Longitude,
                    LogLocation=model.LogLocation,
                    DeviceName=model.DeviceName,
                    DeviceOSVersion=model.DeviceOSVersion,
                    IsCheckInPoint=true,
                    CompanyId=this.CompanyId
                });
            }
            return Ok(response);
        }
        /// <summary>
        /// on checkout time system pick user location and display in google map
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CheckOut(AttendanceEntryModel model)
        {
            var response = _attendance.CheckOut(model);
            if (response.Success)
            {
                _attendance.SaveCheckPoint(new UserMovementLogModel
                {
                    UserId = model.UserId,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    LogLocation = model.LogLocation,
                    DeviceName = model.DeviceName,
                    DeviceOSVersion = model.DeviceOSVersion,
                    IsCheckOutPoint = true,
                    CompanyId = this.CompanyId
                });
            }
            return Ok(response);
        }
        /// <summary>
        /// here checkpoint means I am here.its callin on checkpoint button click event or automaticaly. its depend on employee setup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CheckPoint(AttendanceEntryModel model)
        {
            if (model.Latitude ==null || model.Longitude ==null)
                return Ok(new ResponseModel { Message="Location not found"});

            var response= _attendance.SaveCheckPoint(new UserMovementLogModel
            {
                UserId = model.UserId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                LogLocation = model.LogLocation,
                DeviceName = model.DeviceName,
                DeviceOSVersion = model.DeviceOSVersion,
                CompanyId = this.CompanyId,
                Note=model.Note,
            });
            return Ok(response);
        }
        /// <summary>
        /// company wise today all employees attendance status
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAttendanceFeed()
        {
            var result = _attendance.GetAttendanceFeed(DateTime.UtcNow,this.CompanyId>0?this.CompanyId:(int?)null);
            return Ok(new
            {
                EmployeeList = result,
                StatusCount = new
                {
                    TotalEmployee = result.Count,
                    TotalCheckIn = result.Count(x => x.CheckInTime.HasValue),
                    TotalCheckOut = result.Count(x => x.CheckOutTime.HasValue),
                    TotalNotAttend = result.Count(x => !x.CheckInTime.HasValue)
                }
            });
        }
        /// <summary>
        /// get today attendace for any specific employee
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetMyTodayAttendance()
        {
            var data = _attendance.GetMyTodayAttendance(this.UserId, DateTime.UtcNow);
            if (data == null)
                return Ok(new { EmployeeName =string.Empty});
            return Ok(
                new {
                    data.EmployeeId,
                    data.EmployeeCode,
                    data.EmployeeName,
                    data.Designation,
                    data.DepartmentName,
                    data.CheckInTime,
                    data.CheckOutTime,
                    data.CheckInTimeVw,
                    data.CheckOutTimeVw,
                    data.AttendanceDateVw,
                    data.IsCheckedIn,
                    data.IsCheckedOut,
                    data.ImageFileName,
                    data.Status,
                    data.OfficeStayHour,
                    data.IsLeave,
                    data.IsAutoCheckPoint,
                    data.AutoCheckPointTime,
                    data.CheckInTimeFile,
                    data.CheckOutTimeFile
                });
        }
        [HttpGet]
        public IHttpActionResult GetMyTodayAttendancedetail(string userId)
        {
            var data = _attendance.GetMyTodayAttendance(userId, DateTime.UtcNow);
            if (data == null)
                return Ok(new { EmployeeName = string.Empty });
            return Ok(
                new
                {
                    data.EmployeeId,
                    data.EmployeeCode,
                    data.EmployeeName,
                    data.Designation,
                    data.DepartmentName,
                    data.CheckInTime,
                    data.CheckOutTime,
                    data.CheckInTimeVw,
                    data.CheckOutTimeVw,
                    data.AttendanceDateVw,
                    data.IsCheckedIn,
                    data.IsCheckedOut,
                    data.ImageFileName,
                    data.Status,
                    data.OfficeStayHour,
                    data.IsLeave,
                    data.IsAutoCheckPoint,
                    data.AutoCheckPointTime,
                    data.CheckInTimeFile,
                    data.CheckOutTimeFile
                });
        }


        /// <summary>
        /// here movement details means users all location for specific employee
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetMovementDetails()
        {
            return Ok(_attendance.GetMovementDetails(this.UserId, DateTime.UtcNow).OrderBy(x=>x.LogDateTime));
        }
        [HttpGet]
        public IHttpActionResult GetMovementDetailsinAtand(string userId)
        {
            return Ok(_attendance.GetMovementDetails(userId, DateTime.UtcNow).OrderBy(x => x.LogDateTime));
        }
        [HttpGet]
        public IHttpActionResult GetMovementDetailsAll()
        {
            return Ok(_attendance.GetMovementDetailsAll(DateTime.UtcNow, this.CompanyId > 0 ? this.CompanyId : (int?)null).OrderBy(x => x.LogDateTime));
        }
        [HttpGet]
        public IHttpActionResult GetEmployeeAttendanceFeedWithDate(DateTime startDate,DateTime endDate, string userId)
        {
            userId = userId == "null" ? null : userId;
            var result = _attendance.GetAttendance(userId,startDate,endDate);
            return Ok(new
            {
                EmployeeList = result,
                StatusCount = new
                {
                    TotalEmployee = result.Count,
                    TotalCheckIn = result.Count(x => x.CheckInTime.HasValue),
                    TotalCheckOut = result.Count(x => x.CheckOutTime.HasValue),
                    TotalNotAttend = result.Count(x => !x.CheckInTime.HasValue)
                }
            });
        }
        [HttpGet]
        public IHttpActionResult GetAllEmployeeAttendance(DateTime startdate, DateTime enddate)
        {
            var result = _attendance.GetAttendance(startdate, enddate,this.CompanyId);
            if (result == null)
                return Ok(new { EmployeeName = string.Empty });
            return Ok(new
            {
                EmployeeList = result,
                StatusCount = new
                {
                    TotalEmployee = result.Count,
                    TotalPresent = result.Count(x => x.CheckInTime.HasValue),
                    TotalNotAttend = result.Count(x => !x.CheckInTime.HasValue)
                }
            });
        }

        /// <summary>
        /// for attendance report. month wise
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<AttendanceTotalModel> GetMonthlySummary(string month, int year)
        {
            int monthnumber = DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month;
            var startDate = new DateTime(year, monthnumber, 1);
            var endDate = new DateTime(year, monthnumber, DateTime.DaysInMonth(year, monthnumber));
            var result = _attendance.GetAttendance(startDate, endDate,this.CompanyId);
            if (result == null)
                return new List<AttendanceTotalModel>();

            var aList = new List<AttendanceTotalModel>();
            var empList = result.GroupBy(x => x.UserId).ToList();

            foreach (var e in empList)
            {
                var aModel = new AttendanceTotalModel();
                var employee = result.FirstOrDefault(x => x.UserId == e.Key);
                aModel.EmployeeName = employee.EmployeeName;
                aModel.DepartmentName = employee.DepartmentName;
                aModel.Designation = employee.Designation;
                aModel.ImageFileName = employee.ImageFileName;
                aModel.UserId = e.Key;

                aModel.TotalPresent = result.Count(y => y.UserId == e.Key && y.IsPresent);
                aModel.TotalLeave = result.Count(y => y.UserId == e.Key && y.IsLeave.HasValue && y.IsLeave.Value);

                aModel.TotalCheckedOutMissing = result.Count(y => y.UserId == e.Key && y.NotCheckedOut);
                var totalMinute = result.Where(y => y.UserId == e.Key).Sum(x => x.TotalStayTimeInMinute);
                aModel.TotalStayTime = string.Format("{0}:{1}", totalMinute / 60, totalMinute % 60);

                var officeHourInMin = result.Where(y => y.UserId == e.Key).Sum(x => x.DailyWorkingTimeInMin);
                aModel.TotalOfficeHour = string.Format("{0}:{1}", officeHourInMin / 60, officeHourInMin % 60);

                var overTimeOrDue = totalMinute - officeHourInMin;
                aModel.OvertimeOrDueHour = string.Format("{0}:{1}", overTimeOrDue / 60, overTimeOrDue % 60);

                aList.Add(aModel);
            }

            return aList;
        }

        [HttpGet]
        public IHttpActionResult GetAllEmployeeAttendanceWithMonth(string month, int year)
        {
            var aList = GetMonthlySummary(month, year);
            aList = aList.OrderBy(x => x.EmployeeName).ToList();
            return Ok(aList);
        }
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            var query = _Employee.GetAllEmp().AsQueryable();
            query = query.Where(x => x.WorkingCompanyId == this.CompanyId);
            var result = (from x in query
                          select new
                        {
                            x.PortalUserId,
                            x.Department,
                            x.Designation,
                            x.EmployeeName,
                            
                        });
            return Ok(result);
        }

        /// <summary>
        /// for specific employee get users full month attendance details.leave also display in attendance details report
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetMonthlyAttendanceDetails(string userId, int year, string month)
        {
            int monthnumber = DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month;
            var startDate = new DateTime(year, monthnumber, 1);
            var endDate = new DateTime(year, monthnumber, DateTime.DaysInMonth(year, monthnumber));
            var result = _attendance.GetAttendance(userId,startDate, endDate).OrderBy(x=>x.AttendanceDate);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetUserId(string empCode)
        {

            var user = _attendance.GetUserId(empCode);
            return Ok(new
                {
                    Success = true,
                    UserId = user.Id
                });
            }

        [HttpGet]
        public IHttpActionResult GetMyTodayAttendanceQRcode(string UserId)
        {
            var data = _attendance.GetMyTodayAttendance(UserId, DateTime.UtcNow);
            if (data == null)
                return Ok(new { EmployeeName = string.Empty });
            return Ok(
                new
                {
                    data.EmployeeId,
                    data.EmployeeCode,
                    data.EmployeeName,
                    data.Designation,
                    data.DepartmentName,
                    data.CheckInTime,
                    data.CheckOutTime,
                    data.CheckInTimeVw,
                    data.CheckOutTimeVw,
                    data.AttendanceDateVw,
                    data.IsCheckedIn,
                    data.IsCheckedOut,
                    data.ImageFileName,
                    data.Status,
                    data.OfficeStayHour,
                    data.IsLeave,
                    data.IsAutoCheckPoint,
                    data.AutoCheckPointTime,
                    data.CheckInTimeFile,
                    data.CheckOutTimeFile
                });
        }
        [HttpGet]
        public IHttpActionResult GetMovementDetailsQRcode(string UserId)
        {
            return Ok(_attendance.GetMovementDetails(UserId, DateTime.UtcNow).OrderBy(x => x.LogDateTime));
        }


    }
}
