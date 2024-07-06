using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface IAttendanceReport
    {
        List<AttendanceReportModel> GetAttendanceReports(int? companyId);
        List<AttendanceReportModel> GetUserReports(string userId);
        List<AttendanceReportModel> GetAllUserReports(int? companyId);
    }
}
