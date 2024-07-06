using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.DataAccess.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;


namespace Ems.AttendanceTracking.DataAccess
{
    public class AttendanceReportDataAccess : BaseDatabaseHandler, IAttendanceReport
    {
        public List<AttendanceReportModel> GetAttendanceReports(int? companyId)
        {
            string err = string.Empty;
            string sql = @"select tab.UserId,tab.CompanyId,tab.EmployeeCode,tab.EmployeeName,tab.DepartmentName,tab.DesignationName,tab.AttendanceYear,
                            sum(case when tab.CheckOutTime is null then 1 else 0 end) MissingOutTime,tab.AttendanceMonth,
                            sum(ISNULL(tab.CompletedMinutes,0)) CompletedMinutes,sum(tab.PresentDays) PresentDays,sum(tab.AbsentDays) AbsentDays
                            from (select a.UserId,a.CompanyId,e.EmployeeCode,e.EmployeeName,d.DepartmentName,ds.Name DesignationName,a.CheckOutTime,
                            DATEDIFF(minute, a.CheckInTime, a.CheckOutTime) CompletedMinutes,DATEPART(month, a.AttendanceDate) AttendanceMonth,
                            IIF(isnull(a.IsLeave,0) = 0, 1, 0) PresentDays, IIF(a.IsLeave = 1, 1, 0)AbsentDays,DATEPART(YEAR, a.AttendanceDate) AttendanceYear
                            from Attendance a
                            left join Employee e on e.PortalUserId=a.UserId
                            left join Department d on e.DepartmentId=d.Id
                            left join Designation ds on ds.Id=e.DesignationId) tab
                            where @companyId is null or tab.CompanyId=@companyId
                            group by tab.UserId,tab.CompanyId,tab.EmployeeCode,tab.EmployeeName,tab.DepartmentName,tab.DesignationName,tab.AttendanceMonth,tab.AttendanceYear";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType = DbType.Int32}
                };
            var results = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToAttendanceReportModel);
            return results.Any() ? results : new List<AttendanceReportModel>();
        }

        public List<AttendanceReportModel> GetUserReports(string userId)
        {
            string err = string.Empty;
            string sql = @"select DISTINCT a.UserId,e.EmployeeCode,e.EmployeeName,d.DepartmentName,ds.Name DesignationName,DATEPART(month, a.AttendanceDate) AttendanceMonth,
                            a.CheckInTime,a.CheckOutTime,isnull(DATEDIFF(minute, a.CheckInTime, a.CheckOutTime),0) CompletedMinutes,DATEPART(YEAR, a.AttendanceDate) AttendanceYear,
                            a.LogInLocation,a.LogOutLocation,1 PresentDays,a.deviceName,a.brand,a.modelName 
							from Attendance a
                            left join Employee e on e.PortalUserId=a.UserId
                            left join Department d on e.DepartmentId=d.Id
                            left join Designation ds on ds.Id=e.DesignationId
							where E.EmployeeCode=@userId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue = userId}
                };
            var results = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToAttendanceReportDetailsModel);
            return results.Any() ? results : new List<AttendanceReportModel>();
        }
        public List<AttendanceReportModel> GetAllUserReports(int? companyId)
        {
            //Present days 1 means calculate 8 hours
            string err = string.Empty;
            string sql = @"select DISTINCT a.UserId,e.EmployeeCode,e.EmployeeName,d.DepartmentName,ds.Name DesignationName,DATEPART(month, a.AttendanceDate) AttendanceMonth,
                            a.CheckInTime,a.CheckOutTime,isnull(DATEDIFF(minute, a.CheckInTime, a.CheckOutTime),0) CompletedMinutes,DATEPART(YEAR, a.AttendanceDate) AttendanceYear,
                            a.LogInLocation,a.LogOutLocation,1 PresentDays,a.deviceName,a.brand,a.modelName
							from Attendance a
                            left join Employee e on e.PortalUserId=a.UserId
                            left join Department d on e.DepartmentId=d.Id
                            left join Designation ds on ds.Id=e.DesignationId
                            where @companyId is null or a.CompanyId=@companyId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType = DbType.Int32}
                };
            var results = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToAttendanceReportDetailsModel);
            return results.Any() ? results : new List<AttendanceReportModel>();
        }
    }
}
