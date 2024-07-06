using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Mappers
{
    public class AttendanceMapper
    {
        public static List<EmployeeCountModel> ToEmployeeCountModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeCountModel>();

            while (readers.Read())
            {
                var model = new EmployeeCountModel
                {
                    TotalEmployee = Convert.IsDBNull(readers["TotalEmployee"]) ? (int?)null : Convert.ToInt32(readers["TotalEmployee"]),
                    CompanyName = Convert.IsDBNull(readers["CompanyName"]) ? string.Empty : Convert.ToString(readers["CompanyName"]),
                    WorkingCompanyId = Convert.IsDBNull(readers["WorkingCompanyId"]) ? (int?)null : Convert.ToInt32(readers["WorkingCompanyId"])
                };

                models.Add(model);
            }

            return models;
        }
        public static List<EmployeeDetailsModel> ToExpiryReports(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeDetailsModel>();

            while (readers.Read())
            {
                var model = new EmployeeDetailsModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    EmployeeCode = Convert.ToString(readers["EmployeeCode"]),
                    EmployeeName = Convert.ToString(readers["EmployeeName"]),
                    MobileNo = Convert.IsDBNull(readers["MobileNo"]) ? string.Empty : Convert.ToString(readers["MobileNo"]),
                    PassportNo = Convert.IsDBNull(readers["PassportNo"]) ? string.Empty : Convert.ToString(readers["PassportNo"]),
                    PassportExpiryDateVw = Convert.IsDBNull(readers["PassportExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["PassportExpiryDate"]).ToString(Constants.DateFormat),
                    QID = Convert.IsDBNull(readers["QID"]) ? string.Empty : Convert.ToString(readers["QID"]),
                    QIDExpiryDateVw = Convert.IsDBNull(readers["QIDExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["QIDExpiryDate"]).ToString(Constants.DateFormat),
                    HealthCardExpiryVw = Convert.IsDBNull(readers["HealthCardExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["HealthCardExpiryDate"]).ToString(Constants.DateFormat),
                    VisaNo = Convert.IsDBNull(readers["VisaNo"]) ? string.Empty : Convert.ToString(readers["VisaNo"]),
                    VisaExpirayDate = Convert.IsDBNull(readers["VisaExpirayDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["VisaExpirayDate"]),
                    HealthCardNo = Convert.IsDBNull(readers["HealthCardNo"]) ? string.Empty : Convert.ToString(readers["HealthCardNo"]),
                    HealthCardExpiry = Convert.IsDBNull(readers["HealthCardExpiryDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["HealthCardExpiryDate"])
                };

                models.Add(model);
            }

            return models;
        }
        public static List<AttendanceReportModel> ToAttendanceReportDetailsModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<AttendanceReportModel>();

            while (readers.Read())
            {
                var model = new AttendanceReportModel
                {
                    UserId = Convert.IsDBNull(readers["UserId"]) ? string.Empty : Convert.ToString(readers["UserId"]),
                    LogInLocation = Convert.IsDBNull(readers["LogInLocation"]) ? string.Empty : Convert.ToString(readers["LogInLocation"]),
                    LogOutLocation = Convert.IsDBNull(readers["LogOutLocation"]) ? string.Empty : Convert.ToString(readers["LogOutLocation"]),
                    EmployeeCode = Convert.IsDBNull(readers["EmployeeCode"]) ? string.Empty : Convert.ToString(readers["EmployeeCode"]),
                    EmployeeName = Convert.IsDBNull(readers["EmployeeName"]) ? string.Empty : Convert.ToString(readers["EmployeeName"]),
                    DepartmentName = Convert.IsDBNull(readers["DepartmentName"]) ? string.Empty : Convert.ToString(readers["DepartmentName"]),
                    DesignationName = Convert.IsDBNull(readers["DesignationName"]) ? string.Empty : Convert.ToString(readers["DesignationName"]),
                    CheckInTime = Convert.IsDBNull(readers["CheckInTime"]) ? (DateTime?)null : Convert.ToDateTime(readers["CheckInTime"]),
                    CheckOutTime = Convert.IsDBNull(readers["CheckOutTime"]) ? (DateTime?)null : Convert.ToDateTime(readers["CheckOutTime"]),
                    CompletedMinutes = Convert.ToDecimal(readers["CompletedMinutes"]),
                    AttendanceMonth = Convert.ToInt32(readers["AttendanceMonth"]),
                    AttendanceYear = Convert.ToInt32(readers["AttendanceYear"]),
                    PresentDays = Convert.ToInt32(readers["PresentDays"]),

                    DeviceName = Convert.IsDBNull(readers["deviceName"]) ? string.Empty : Convert.ToString(readers["deviceName"]),
                    DeviceBrand = Convert.IsDBNull(readers["brand"]) ? string.Empty : Convert.ToString(readers["brand"]),
                    DeviceModelName = Convert.IsDBNull(readers["modelName"]) ? string.Empty : Convert.ToString(readers["modelName"])
                };

                models.Add(model);
            }

            return models;
        }
        public static List<AttendanceReportModel> ToAttendanceReportModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<AttendanceReportModel>();

            while (readers.Read())
            {
                var model = new AttendanceReportModel
                {
                    UserId = Convert.IsDBNull(readers["UserId"]) ? string.Empty : Convert.ToString(readers["UserId"]),
                    EmployeeCode = Convert.IsDBNull(readers["EmployeeCode"]) ? string.Empty : Convert.ToString(readers["EmployeeCode"]),
                    EmployeeName = Convert.IsDBNull(readers["EmployeeName"]) ? string.Empty : Convert.ToString(readers["EmployeeName"]),
                    DepartmentName = Convert.IsDBNull(readers["DepartmentName"]) ? string.Empty : Convert.ToString(readers["DepartmentName"]),
                    DesignationName = Convert.IsDBNull(readers["DesignationName"]) ? string.Empty : Convert.ToString(readers["DesignationName"]),
                    PresentDays = Convert.ToInt32(readers["PresentDays"]),
                    AbsentDays = Convert.ToInt32(readers["AbsentDays"]),
                    AttendanceMonth = Convert.ToInt32(readers["AttendanceMonth"]),
                    AttendanceYear = Convert.ToInt32(readers["AttendanceYear"]),
                    CompletedMinutes = Convert.ToDecimal(readers["CompletedMinutes"]),
                    MissingOutTime = Convert.IsDBNull(readers["MissingOutTime"]) ? (int?)null : Convert.ToInt32(readers["MissingOutTime"]),
                };

                models.Add(model);
            }

            return models;
        }
        public static List<ExpiredSummeryModel> ToExpiredSummeryModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<ExpiredSummeryModel>();

            while (readers.Read())
            {
                var model = new ExpiredSummeryModel
                {
                    QID = Convert.IsDBNull(readers["QID"]) ? string.Empty : Convert.ToString(readers["QID"]),
                    VisaNo = Convert.IsDBNull(readers["VisaNo"]) ? string.Empty : Convert.ToString(readers["VisaNo"]),
                    PassportNo = Convert.IsDBNull(readers["PassportNo"]) ? string.Empty : Convert.ToString(readers["PassportNo"]),
                    HealthCardNo = Convert.IsDBNull(readers["HealthCardNo"]) ? string.Empty : Convert.ToString(readers["HealthCardNo"])
                };

                models.Add(model);
            }

            return models;
        }
        public static List<EmployeeLeaveStatusModel> LeaveStatusModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeLeaveStatusModel>();

            while (readers.Read())
            {
                var model = new EmployeeLeaveStatusModel
                {
                    CompanyId = Convert.ToInt32(readers["CompanyId"]),
                    CompanyName = Convert.IsDBNull(readers["CompanyName"]) ? string.Empty : Convert.ToString(readers["CompanyName"]),
                    OnLeave = Convert.IsDBNull(readers["OnLeave"]) ? (int?)null : Convert.ToInt32(readers["OnLeave"]),
                };

                models.Add(model);
            }

            return models;
        }
        public static List<EmployeeStatusSummeryModel> ToStatusSummeryModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeStatusSummeryModel>();

            while (readers.Read())
            {
                var model = new EmployeeStatusSummeryModel
                {
                    StatusId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                    EmployeeCount = Convert.ToInt32(readers["EmployeeCount"])
                };

                models.Add(model);
            }

            return models;
        }
        public static List<AttendanceModel> ToAttendance(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<AttendanceModel>();

            while (readers.Read())
            {
                var model = new AttendanceModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    EmployeeId = Convert.IsDBNull(readers["EmployeeId"]) ? (int?)null : Convert.ToInt32(readers["EmployeeId"]),
                    UserId = Convert.IsDBNull(readers["UserId"]) ? string.Empty : Convert.ToString(readers["UserId"]),
                    EmployeeName = Convert.IsDBNull(readers["EmployeeName"]) ? string.Empty : Convert.ToString(readers["EmployeeName"]),
                    PhoneNumber = Convert.IsDBNull(readers["PhoneNumber"]) ? string.Empty : Convert.ToString(readers["PhoneNumber"]),
                    AttendanceDate = Convert.IsDBNull(readers["AttendanceDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["AttendanceDate"]),
                    CheckInTime = Convert.IsDBNull(readers["CheckInTime"]) ? (DateTime?)null : Convert.ToDateTime(readers["CheckInTime"]),
                    CheckOutTime = Convert.IsDBNull(readers["CheckOutTime"]) ? (DateTime?)null : Convert.ToDateTime(readers["CheckOutTime"]),
                    LessTimeReason = Convert.IsDBNull(readers["LessTimeReason"]) ? string.Empty : Convert.ToString(readers["LessTimeReason"]),
                    DepartmentName = Convert.IsDBNull(readers["DepartmentName"]) ? string.Empty : Convert.ToString(readers["DepartmentName"]),
                    Designation = Convert.IsDBNull(readers["Designation"]) ? string.Empty : Convert.ToString(readers["Designation"]),
                    ImageFileName = Convert.IsDBNull(readers["ImageFileName"]) ? string.Empty : Convert.ToString(readers["ImageFileName"]),
                    DailyWorkingTimeInMin = Convert.IsDBNull(readers["DailyWorkingTimeInMin"]) ? (int?)null : Convert.ToInt32(readers["DailyWorkingTimeInMin"]),

                    AllowOfficeLessTimeInMin = Convert.IsDBNull(readers["AllowOfficeLessTime"]) ? (int?)null : Convert.ToInt32(readers["AllowOfficeLessTime"]),
                    IsLeave = Convert.IsDBNull(readers["IsLeave"]) ? (bool?)null : Convert.ToBoolean(readers["IsLeave"]),
                    IsAutoCheckPoint = Convert.IsDBNull(readers["IsAutoCheckPoint"]) ? (bool?)null : Convert.ToBoolean(readers["IsAutoCheckPoint"]),
                    AutoCheckPointTime = Convert.IsDBNull(readers["AutoCheckPointTime"]) ? string.Empty : Convert.ToString(readers["AutoCheckPointTime"]),
                    EmployeeCode = Convert.IsDBNull(readers["EmployeeCode"]) ? string.Empty : Convert.ToString(readers["EmployeeCode"]),
                    PunchInDeviceName = Convert.IsDBNull(readers["deviceName"]) ? string.Empty : Convert.ToString(readers["deviceName"]),
                    LogInLocation = Convert.IsDBNull(readers["deviceName"]) ? string.Empty : Convert.ToString(readers["LogInLocation"]),
                    LogOutLocation = Convert.IsDBNull(readers["deviceName"]) ? string.Empty : Convert.ToString(readers["LogOutLocation"])
                };

                model.ImagePath = Constants.LocalFilePath + model.ImageFileName;

                models.Add(model);
            }

            return models;
        }
        public static List<UserMovementLogModel> ToMovementLog(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<UserMovementLogModel>();

            while (readers.Read())
            {
                var model = new UserMovementLogModel
                {
                    Id = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"]),
                    UserId = Convert.IsDBNull(readers["UserId"]) ? string.Empty : Convert.ToString(readers["UserId"]),
                    LogDateTime = Convert.IsDBNull(readers["LogDateTime"]) ? (DateTime?)null : Convert.ToDateTime(readers["LogDateTime"]),
                    Latitude = Convert.IsDBNull(readers["Latitude"]) ? (decimal?)null : Convert.ToDecimal(readers["Latitude"]),
                    Longitude = Convert.IsDBNull(readers["Longitude"]) ? (decimal?)null : Convert.ToDecimal(readers["Longitude"]),
                    LogLocation = Convert.IsDBNull(readers["LogLocation"]) ? string.Empty : Convert.ToString(readers["LogLocation"]),
                    DeviceName = Convert.IsDBNull(readers["DeviceName"]) ? string.Empty : Convert.ToString(readers["DeviceName"]),
                    DeviceOSVersion = Convert.IsDBNull(readers["DeviceOSVersion"]) ? string.Empty : Convert.ToString(readers["DeviceOSVersion"]),
                    IsCheckInPoint = Convert.IsDBNull(readers["IsCheckInPoint"]) ? (bool?)null : Convert.ToBoolean(readers["IsCheckInPoint"]),
                    IsCheckOutPoint = Convert.IsDBNull(readers["IsCheckOutPoint"]) ? (bool?)null : Convert.ToBoolean(readers["IsCheckOutPoint"]),
                    Note = Convert.IsDBNull(readers["Note"]) ? string.Empty : Convert.ToString(readers["Note"]),
                };

                models.Add(model);
            }

            return models;
        }
        public static List<UserMovementLogModel> ToMovementLogAll(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<UserMovementLogModel>();

            while (readers.Read())
            {
                var model = new UserMovementLogModel
                {
                    Id = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"]),
                    UserId = Convert.IsDBNull(readers["UserId"]) ? string.Empty : Convert.ToString(readers["UserId"]),
                    UserName = Convert.IsDBNull(readers["UserName"]) ? string.Empty : Convert.ToString(readers["UserName"]),
                    LogDateTime = Convert.IsDBNull(readers["LogDateTime"]) ? (DateTime?)null : Convert.ToDateTime(readers["LogDateTime"]),
                    Latitude = Convert.IsDBNull(readers["Latitude"]) ? (decimal?)null : Convert.ToDecimal(readers["Latitude"]),
                    Longitude = Convert.IsDBNull(readers["Longitude"]) ? (decimal?)null : Convert.ToDecimal(readers["Longitude"]),
                    LogLocation = Convert.IsDBNull(readers["LogLocation"]) ? string.Empty : Convert.ToString(readers["LogLocation"]),
                    DeviceName = Convert.IsDBNull(readers["DeviceName"]) ? string.Empty : Convert.ToString(readers["DeviceName"]),
                    DeviceOSVersion = Convert.IsDBNull(readers["DeviceOSVersion"]) ? string.Empty : Convert.ToString(readers["DeviceOSVersion"]),
                    IsCheckInPoint = Convert.IsDBNull(readers["IsCheckInPoint"]) ? (bool?)null : Convert.ToBoolean(readers["IsCheckInPoint"]),
                    IsCheckOutPoint = Convert.IsDBNull(readers["IsCheckOutPoint"]) ? (bool?)null : Convert.ToBoolean(readers["IsCheckOutPoint"]),
                    Note = Convert.IsDBNull(readers["Note"]) ? string.Empty : Convert.ToString(readers["Note"]),

                };

                models.Add(model);
            }

            return models;
        }
        public static List<UserCredentialModel> TompId(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<UserCredentialModel>();

            while (readers.Read())
            {
                var model = new UserCredentialModel
                {
                    Id = Convert.IsDBNull(readers["PortalUserId"]) ? string.Empty : Convert.ToString(readers["PortalUserId"]),
              
                };

                models.Add(model);
            }

            return models;
        }
    }
}
