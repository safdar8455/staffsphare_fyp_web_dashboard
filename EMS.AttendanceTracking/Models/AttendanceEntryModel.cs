using Ems.BusinessTracker.Common;
using System;
using System.Web.Script.Serialization;

namespace Ems.AttendanceTracking.Models
{
    public class AttendanceEntryModel
    {
        public string UserId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string LogLocation { get; set; }
        public string DeviceName { get; set; }
        public string DeviceOSVersion { get; set; }
        public string brand { get; set; }
        public string modelName { get; set; }
        public string osName { get; set; }
        public string Reason { get; set; }

        public int? OfficeHour { get; set; }
        public int? AllowOfficeLessTime { get; set; }
        public bool? IsLeave { get; set; }

        public string CheckInTimeFile { get; set; }
        public string CheckOutTimeFile { get; set; }
        public int? CompanyId { get; set; }
        public string Note { get; set; }

    }
    public class AttendanceTotalModel
    {
        public string UserId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public string ImageFileName { get; set; }
        public int? TotalPresent { get; set; }
        public int? TotalCheckedOutMissing { get; set; }
        public string TotalStayTime { get; set; }
        public string TotalOfficeHour { get; set; }
        public string OvertimeOrDueHour { get; set; }
        public int? TotalLeave { get; set; }
        public double TotalScore { get; set; }
    }
    public class AttendanceModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ScriptIgnore]
        public DateTime? AttendanceDate { get; set; }
        [ScriptIgnore]
        public DateTime? CheckInTime { get; set; }
        [ScriptIgnore]
        public DateTime? CheckOutTime { get; set; }
        public bool? IsLeave { get; set; }


        public string CheckInTimeFile { get; set; }
        public string CheckOutTimeFile { get; set; }

        public string PunchInDeviceName { get; set; }
        public string LogInLocation { get; set; }
        public string LogOutLocation { get; set; }

        public string LessTimeReason { get; set; }
        public int? DailyWorkingTimeInMin { get; set; }
        public int? AllowOfficeLessTimeInMin { get; set; }

        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PhoneNumber { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public bool? IsAutoCheckPoint { get; set; }
        public string AutoCheckPointTime { get; set; }
        public string EmployeeCode { get; set; }
        public string ImagePath { get; set; }
        public string AttendanceDateVw
        {
            get { return AttendanceDate.HasValue ? AttendanceDate.Value.ToZoneTime().ToString(Constants.DateLongFormat) : string.Empty; }
        }

        public string AttendancceDayName
        {
            get { return AttendanceDate.HasValue ? AttendanceDate.Value.ToZoneTime().ToString("ddd") : string.Empty; }
        }

        public string AttendancceDayNumber
        {
            get { return AttendanceDate.HasValue ? string.Format("{0}", AttendanceDate.Value.ToZoneTime().Day) : string.Empty; }
        }

        public string CheckInTimeVw
        {
            get { return CheckInTime.HasValue ? CheckInTime.Value.ToZoneTime().ToString(Constants.TimeFormat) : (IsLeave.HasValue && IsLeave.Value ? "Leave" : string.Empty); }
        }
        public string CheckOutTimeVw
        {
            get { return CheckOutTime.HasValue ? CheckOutTime.Value.ToZoneTime().ToString(Constants.TimeFormat) : string.Empty; }
        }

        public string OfficeStayHour
        {
            get
            {
                if (!CheckInTime.HasValue)
                    return string.Empty;
                TimeSpan result = CheckOutTime.HasValue ? CheckOutTime.Value.ToZoneTime().Subtract(CheckInTime.Value.ToZoneTime()) : DateTime.UtcNow.ToZoneTime().Subtract(CheckInTime.Value.ToZoneTime());
                int hours = result.Hours;
                int minutes = result.Minutes;

                return string.Format("{0}:{1}", hours, minutes);
            }
        }

        public bool IsCheckedIn
        {
            get
            {
                return CheckInTime.HasValue && !CheckOutTime.HasValue;
            }
        }

        public bool IsPresent
        {
            get
            {
                return CheckInTime.HasValue;
            }
        }
        public bool IsCheckedOut
        {
            get
            {
                return CheckInTime.HasValue && CheckOutTime.HasValue;
            }
        }

        public bool NotCheckedOut
        {
            get
            {
                return CheckInTime.HasValue && !CheckOutTime.HasValue;
            }
        }

        public bool NotAttend
        {
            get
            {
                return !CheckInTime.HasValue && !CheckOutTime.HasValue;
            }
        }

        public string ImageFileName { get; set; }
        public string Status
        {
            get
            {
                if (CheckInTime.HasValue && !CheckOutTime.HasValue)
                    return "Checked-In";
                else if (CheckInTime.HasValue && CheckOutTime.HasValue)
                    return "Checked-Out";
                else if (!CheckInTime.HasValue && !CheckOutTime.HasValue && !IsLeave.HasValue)
                    return "Absent";
                else if (!CheckInTime.HasValue && !CheckOutTime.HasValue && IsLeave.HasValue && IsLeave.Value)
                    return "Leave";
                else
                    return string.Empty;
            }
        }
        public int? TotalStayTimeInMinute
        {
            get
            {
                if (!CheckInTime.HasValue)
                    return 0;
                TimeSpan result = CheckOutTime.HasValue ? CheckOutTime.Value.ToZoneTime().Subtract(CheckInTime.Value.ToZoneTime()) : CheckInTime.Value.ToZoneTime().Subtract(CheckInTime.Value.ToZoneTime());
                int hours = result.Hours;
                int minutes = result.Minutes;

                return hours * 60 + minutes;
            }
        }

    }
    public class EmployeeLeaveStatusModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? OnLeave { get; set; }
    }
    public class EmployeeStatusSummeryModel
    {
        public string EmployeeStatus
        {
            get
            {
                return StatusId.HasValue ? EnumUtility.GetDescriptionFromEnumValue((EmployeeStatus)StatusId) : string.Empty;
            }
        }
        public int EmployeeCount { get; set; }
        public int? StatusId { get; set; }
        public int Total { get; set; }
        public decimal CountPercent { get; set; }
        public string CountPercentString { get; set; }
    }
    public class ExpiredSummeryModel
    {
        public string QID { get; set; }
        public string VisaNo { get; set; }
        public string PassportNo { get; set; }
        public string HealthCardNo { get; set; }
    }
    public class EmployeeCountModel
    {
        public int? WorkingCompanyId { get; set; }
        public int? TotalEmployee { get; set; }
        public string CompanyName { get; set; }
    }
    public class UserMovementLogModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime? LogDateTime { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string LogLocation { get; set; }
        public bool? IsCheckInPoint { get; set; }
        public bool? IsCheckOutPoint { get; set; }
        public bool? IsCheckPoint { get; set; }
        public string DeviceName { get; set; }
        public string DeviceOSVersion { get; set; }
        public string Note { get; set; }
        public string LogTimeVw
        {
            get { return LogDateTime.HasValue ? LogDateTime.Value.ToZoneTime().ToString(Constants.TimeFormat) : string.Empty; }
        }
        public int? CompanyId { get; set; }
    }
}
