using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface IAttendance
    {
        ResponseModel CheckIn(AttendanceEntryModel model);
        ResponseModel AddAttendanceAsLeave(AttendanceEntryModel model);
        ResponseModel CheckOut(AttendanceEntryModel model);
        ResponseModel SaveCheckPoint(UserMovementLogModel model);
        List<AttendanceModel> GetAttendanceFeed(DateTime date, int? companyId);
        List<EmployeeDetailsModel> GetExpiryReports(int expiryId, int? companyId);
        List<EmployeeLeaveStatusModel> GetLeaveStatusList(DateTime date, int? companyId);
        List<EmployeeCountModel> GetEmployeeCount(int? companyId);
        List<EmployeeStatusSummeryModel> GetStatusSummery(int? companyId);
        List<ExpiredSummeryModel> GetExpiredSummery(int? companyId);
        List<AttendanceModel> GetAttendance(DateTime startDate, DateTime endDate,int companyId);
        List<AttendanceModel> GetAttendance(string userId, DateTime startDate, DateTime endDate);
        List<UserMovementLogModel> GetMovementDetails(string userId, DateTime date);
        List<UserMovementLogModel> GetMovementDetailsAll(DateTime date,int? companyId);
        AttendanceModel GetMyTodayAttendance(string userId, DateTime date);
        UserCredentialModel GetUserId(string empCode);
    }
}
