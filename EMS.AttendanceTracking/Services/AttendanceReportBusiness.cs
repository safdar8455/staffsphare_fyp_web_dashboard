using System.Collections.Generic;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Services
{
	public class AttendanceReportBusiness
	{
		private readonly IAttendanceReport _attendanceReport;
        private readonly IEmployee _Employee;
        public AttendanceReportBusiness()
		{
			_attendanceReport = AttendanceUnityMapper.GetInstance<IAttendanceReport>();
            _Employee = AttendanceUnityMapper.GetInstance<IEmployee>();
        }
		public List<AttendanceReportModel> GetAttendanceReports(int? companyId)
		{
			var list = _attendanceReport.GetAttendanceReports(companyId);
			return list;
		}
		public List<AttendanceReportModel> GetUserReports(string userId)
		{
			var list = _attendanceReport.GetUserReports(userId);
			return list;
		}
		public List<AttendanceReportModel> GetAllUserReports(int? companyId)
		{
			var list = _attendanceReport.GetAllUserReports(companyId);
			return list;
		}
		public List<EmployeeExportModel> GetQrCodeEmployee()
        {
            var list = _Employee.GetAll();
            return list;
        }
    }
}
