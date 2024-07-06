using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.DataAccess
{
    public class AttendanceDataAccess : BaseDatabaseHandler, IAttendance
    {
        public ResponseModel CheckIn(AttendanceEntryModel model)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserId", ParamValue =model.UserId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LogLocation", ParamValue =model.LogLocation},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@AttendanceDate", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CheckInTime", ParamValue = DateTime.UtcNow,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@DailyWorkingTimeInMin", ParamValue = model.OfficeHour,DBType=DbType.Int32},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@AllowOfficeLessTime", ParamValue = model.AllowOfficeLessTime,DBType=DbType.Int32},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyId", ParamValue =model.CompanyId,DBType=DbType.Int32},

                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@deviceName", ParamValue =model.DeviceName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@brand", ParamValue =model.brand},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@modelName", ParamValue =model.modelName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@osName", ParamValue =model.osName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@osVersion", ParamValue =model.DeviceOSVersion},

                };

            const string sql = @"IF NOT EXISTS(SELECT * FROM Attendance A WHERE A.UserId=@UserId AND CONVERT(DATE,AttendanceDate)=CONVERT(DATE,@AttendanceDate))
                                BEGIN
								    IF @CheckInTime is not null
								    BEGIN
                                    INSERT INTO Attendance(UserId,LogInLocation,AttendanceDate,CheckInTime,DailyWorkingTimeInMin,AllowOfficeLessTime,CompanyId,deviceName,brand,modelName,osName,osVersion)
				                           VALUES(@UserId,@LogLocation,@AttendanceDate,@CheckInTime,@DailyWorkingTimeInMin,@AllowOfficeLessTime,@CompanyId,@deviceName,@brand,@modelName,@osName,@osVersion)
                                    END
								END";
            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }

        public ResponseModel AddAttendanceAsLeave(AttendanceEntryModel model)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserId", ParamValue =model.UserId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@AttendanceDate", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsLeave", ParamValue =true,DBType=DbType.Boolean},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyId", ParamValue =model.CompanyId,DBType=DbType.Int32}
                };

            const string sql = @"IF NOT EXISTS(SELECT * FROM Attendance A WHERE A.UserId=@UserId AND CONVERT(DATE,AttendanceDate)=CONVERT(DATE,@AttendanceDate))
                                BEGIN
                                INSERT INTO Attendance(UserId,AttendanceDate,IsLeave,CompanyId)
				                    VALUES(@UserId,@AttendanceDate,@IsLeave,@CompanyId)
                                END";
            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }

        public ResponseModel CheckOut(AttendanceEntryModel model)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserId", ParamValue =model.UserId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LogLocation", ParamValue =model.LogLocation},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LessTimeReason", ParamValue =model.Reason},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CheckOutTime", ParamValue = DateTime.UtcNow,DBType=DbType.DateTime}
                };

            const string sql = @"DECLARE @id bigint
                                SELECT TOP 1 @id=Id FROM Attendance WHERE UserId=@UserId AND CheckOutTime IS NULL ORDER BY Id DESC
                                UPDATE Attendance SET CheckOutTime=@CheckOutTime,LogOutLocation=@LogLocation,LessTimeReason=@LessTimeReason WHERE Id=@id";
            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }

        public ResponseModel SaveCheckPoint(UserMovementLogModel model)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =Guid.NewGuid().ToString()},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserId", ParamValue =model.UserId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LogDateTime", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime2},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Latitude", ParamValue =model.Latitude},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Longitude", ParamValue =model.Longitude},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LogLocation", ParamValue =model.LogLocation},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsCheckInPoint", ParamValue =model.IsCheckInPoint},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsCheckOutPoint", ParamValue =model.IsCheckOutPoint},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@DeviceName", ParamValue =model.DeviceName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@DeviceOSVersion", ParamValue =model.DeviceOSVersion},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyId", ParamValue =model.CompanyId,DBType=DbType.Int32},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Note", ParamValue =model.Note},
                };

            const string sql = @"INSERT INTO UserMovementLog(Id,UserId,LogDateTime,Latitude,Longitude,LogLocation,IsCheckInPoint,
                                    IsCheckOutPoint,DeviceName,DeviceOSVersion,CompanyId,Note)
				                 VALUES(@Id,@UserId,@LogDateTime,@Latitude,@Longitude,@LogLocation,@IsCheckInPoint,
                                    @IsCheckOutPoint,@DeviceName,@DeviceOSVersion,@CompanyId,@Note)";
            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }

        public List<AttendanceModel> GetAttendanceFeed(DateTime date, int? companyId)
        {
            const string sql = @"
                                SELECT c.Id,t.UserId,t.AttendanceDate,t.CheckInTime,t.CheckOutTime,t.AllowOfficeLessTime,T.IsLeave,c.EmployeeCode,
                                t.LessTimeReason,t.DailyWorkingTimeInMin,C.Id EmployeeId
                                ,C.EmployeeName,DESI.Name Designation,d.DepartmentName,c.ImageFileName,c.MobileNo PhoneNumber,
                                null IsAutoCheckPoint,null AutoCheckPointTime, null TotalHours, null TotalMinutes,t.deviceName,t.LogInLocation,t.LogOutLocation
                                FROM Attendance t
                                LEFT JOIN Employee C ON T.UserId=C.PortalUserId 
                                LEFT JOIN UserCredentials CR ON C.PortalUserId=CR.Id
                                left join Department d on c.DepartmentId=d.Id
                                LEFT JOIN Designation DESI ON C.DesignationId=DESI.Id
                                where convert(date,t.AttendanceDate)=convert(date,@date)  
                                AND (@companyId is null or C.WorkingCompanyId=@companyId)
                                UNION ALL

                                SELECT t.Id,t.PortalUserId UserId,@date AttendanceDate,NULL CheckInTime,NULL CheckOutTime,NULL AllowOfficeLessTime,NULL IsLeave,t.EmployeeCode,
                                NULL LessTimeReason,NULL DailyWorkingTimeInMin,t.Id EmployeeId
                                ,t.EmployeeName,DESI.Name Designation,d.DepartmentName,t.ImageFileName,t.MobileNo PhoneNumber,null IsAutoCheckPoint,null AutoCheckPointTime, null TotalHours, null TotalMinutes,null deviceName ,null LogInLocation,null LogOutLocation
                                FROM Employee t
                                LEFT JOIN UserCredentials CR ON T.PortalUserId=CR.Id
                                left join Department d on t.DepartmentId=d.Id
                                LEFT JOIN Designation DESI ON t.DesignationId=DESI.Id
                                where (@companyId is null or T.WorkingCompanyId=@companyId) and t.PortalUserId NOT in (
                                SELECT T.UserId FROM Attendance t
                                where convert(date,t.AttendanceDate)=convert(date,@date)
                                )";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@date", ParamValue = date,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType=DbType.Int32}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToAttendance);
            return data;
        }

        public List<AttendanceModel> GetAttendance(DateTime startDate, DateTime endDate, int companyId)
        {
            const string sql = @"
                                 SELECT c.Id,t.UserId,t.AttendanceDate,t.CheckInTime,t.CheckOutTime,t.AllowOfficeLessTime,T.IsLeave,c.EmployeeCode,
                                t.LessTimeReason,t.DailyWorkingTimeInMin,C.Id EmployeeId
                                ,C.EmployeeName,DESI.Name Designation,d.DepartmentName,c.ImageFileName,c.MobileNo PhoneNumber,
                                null IsAutoCheckPoint,null AutoCheckPointTime, null TotalHours, null TotalMinutes,t.deviceName ,t.LogInLocation,t.LogOutLocation
                                FROM Attendance t
                                LEFT JOIN Employee C ON T.UserId=C.PortalUserId 
                                LEFT JOIN UserCredentials CR ON C.PortalUserId=CR.Id
                                left join Department d on c.DepartmentId=d.Id
                                LEFT JOIN Designation DESI ON C.DesignationId=DESI.Id
                                where t.companyId=@companyId and (CONVERT(DATE,AttendanceDate) BETWEEN convert(date,@startDate) AND convert(date,@endDate))";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@startDate", ParamValue = startDate,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@endDate", ParamValue = endDate,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType=DbType.Int32}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToAttendance);
            return data;
        }

        public List<EmployeeCountModel> GetEmployeeCount(int? companyId)
        {
            const string sql = @"select e.WorkingCompanyId,c.CompanyName,count (e.Id) TotalEmployee
                                    from Employee e
                                    left join Company c on c.Id=e.WorkingCompanyId
                                    where (@companyId is null or e.WorkingCompanyId=@companyId)
                                    group by e.WorkingCompanyId,c.CompanyName";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType=DbType.Int32}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToEmployeeCountModel);
            return data;
        }
        public List<ExpiredSummeryModel> GetExpiredSummery(int? companyId)
        {
            const string sql = @"select e.QID,e.VisaNo,e.PassportNo,e.HealthCardNo
                                from Employee e
                                where (@companyId is null or e.WorkingCompanyId=@companyId) AND (e.QIDExpiryDate>GETDATE() OR e.VisaExpirayDate>GETDATE()
                                OR e.PassportExpiryDate>GETDATE() OR e.HealthCardExpiryDate>GETDATE())";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType=DbType.Int32}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToExpiredSummeryModel);
            return data;
        }
        public List<AttendanceModel> GetAttendance(string userId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                                 SELECT c.Id,t.UserId,t.AttendanceDate,t.CheckInTime,t.CheckOutTime,t.AllowOfficeLessTime,T.IsLeave,c.EmployeeCode,
                                t.LessTimeReason,t.DailyWorkingTimeInMin,C.Id EmployeeId
                                ,C.EmployeeName,DESI.Name Designation,d.DepartmentName,c.ImageFileName,c.MobileNo PhoneNumber,
                                null IsAutoCheckPoint,null AutoCheckPointTime, null TotalHours, null TotalMinutes,t.deviceName ,t.LogInLocation,t.LogOutLocation
                                FROM Attendance t
                                LEFT JOIN Employee C ON T.UserId=C.PortalUserId 
                                LEFT JOIN UserCredentials CR ON C.PortalUserId=CR.Id
                                left join Department d on c.DepartmentId=d.Id
                                LEFT JOIN Designation DESI ON C.DesignationId=DESI.Id
                                where  T.UserId=@userId and (CONVERT(DATE,AttendanceDate) BETWEEN convert(date,@startDate) AND convert(date,@endDate))";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue = userId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@startDate", ParamValue = startDate,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@endDate", ParamValue = endDate,DBType=DbType.DateTime}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToAttendance);
            return data;
        }

        public AttendanceModel GetMyTodayAttendance(string userId, DateTime date)
        {
            const string sql = @"IF EXISTS(SELECT TOP 1 * FROM Attendance A WHERE A.UserId=@userId and convert(date,A.AttendanceDate)=convert(date,@date))
                                BEGIN
	                            SELECT c.Id,t.UserId,t.AttendanceDate,t.CheckInTime,t.CheckOutTime,t.AllowOfficeLessTime,T.IsLeave,c.EmployeeCode,
                                t.LessTimeReason,t.DailyWorkingTimeInMin,C.Id EmployeeId
                                ,C.EmployeeName,DESI.Name Designation,d.DepartmentName,c.ImageFileName,c.MobileNo PhoneNumber,
                                null IsAutoCheckPoint,null AutoCheckPointTime, null TotalHours, null TotalMinutes,t.deviceName ,t.LogInLocation,t.LogOutLocation
                                FROM Attendance t
                                LEFT JOIN Employee C ON T.UserId=C.PortalUserId 
                                LEFT JOIN UserCredentials CR ON C.PortalUserId=CR.Id
                                left join Department d on c.DepartmentId=d.Id
                                LEFT JOIN Designation DESI ON C.DesignationId=DESI.Id
	                            where t.UserId=@userId and convert(date,t.AttendanceDate)=convert(date,@date)
                                END
                                ELSE
                                BEGIN
                                 SELECT t.Id,t.PortalUserId UserId,@date AttendanceDate,NULL CheckInTime,NULL CheckOutTime,NULL AllowOfficeLessTime,NULL IsLeave,t.EmployeeCode,
                                NULL LessTimeReason,NULL DailyWorkingTimeInMin,t.Id EmployeeId
                                ,t.EmployeeName,DESI.Name Designation,d.DepartmentName,t.ImageFileName,t.MobileNo PhoneNumber,null IsAutoCheckPoint,null AutoCheckPointTime, null TotalHours, null TotalMinutes ,null deviceName,null LogInLocation,null LogOutLocation
                                FROM Employee t
                                LEFT JOIN UserCredentials CR ON T.PortalUserId=CR.Id
                                left join Department d on t.DepartmentId=d.Id
                                LEFT JOIN Designation DESI ON t.DesignationId=DESI.Id
                                where t.PortalUserId=@userId
                                END";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue = userId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@date", ParamValue = date,DBType=DbType.DateTime}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToAttendance);
            return (data != null && data.Count > 0) ? data.FirstOrDefault() : null;
        }

        public List<UserMovementLogModel> GetMovementDetails(string userId, DateTime date)
        {
            const string sql = @"SELECT T.* FROM UserMovementLog t
                                where t.UserId=@userId and convert(date,t.LogDateTime)=convert(date,@date)";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue = userId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@date", ParamValue = date,DBType=DbType.DateTime}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToMovementLog);
            return data;
        }
        public List<UserMovementLogModel> GetMovementDetailsAll(DateTime date, int? companyId)
        {
            const string sql = @"select a.*,e.FullName UserName from UserMovementLog a
                                right join (select UserId,max(LogDateTime)LogDateTime from UserMovementLog  where convert(date,LogDateTime)=convert(date,@date) group by UserId)  t on t.UserId=a.UserId and t.LogDateTime=a.LogDateTime
                                left join UserCredentials e on a.UserId=e.Id WHERE (@companyId is null or e.Companyid=@companyId)";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@date", ParamValue = date,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType=DbType.Int32}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToMovementLogAll);
            return data;
        }

        public List<EmployeeLeaveStatusModel> GetLeaveStatusList(DateTime date, int? companyId)
        {
            const string sql = @"select c.Id CompanyId,c.CompanyName,count(a.IsLeave)OnLeave
                                    from Attendance a
                                    left join Company c on c.Id=a.CompanyId
                                    left join Employee e on e.PortalUserId=a.UserId
                                    WHERE (@companyId is null or a.Companyid=@companyId) AND convert(date,a.AttendanceDate)=convert(date,@date)
                                    group by c.Id,c.CompanyName";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@date", ParamValue = date,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType=DbType.Int32}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.LeaveStatusModel);
            return data;
        }
        public List<EmployeeStatusSummeryModel> GetStatusSummery(int? companyId)
        {
            const string sql = @"select e.StatusId,COUNT(e.EmployeeCode) EmployeeCount
                                    from Employee e
									where (@companyId is null or e.WorkingCompanyId=@companyId)
                                    group by e.StatusId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId,DBType=DbType.Int32}
                };
            var data = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.ToStatusSummeryModel);
            return data;
        }
        public UserCredentialModel GetUserId(string empCode)
        {

            const string sql = @"select PortalUserId from Employee
                                WHERE QrCodeNo=@empCode";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@empCode", ParamValue = empCode},
                };
            var results = ExecuteDBQuery(sql, queryParamList, AttendanceMapper.TompId);
            return results.Any() ? results.FirstOrDefault() : null;

        }

        public List<EmployeeDetailsModel> GetExpiryReports(int expiryId, int? companyId)
        {
            var param = new QueryParamList
                            {
                                new QueryParamObj{ParamName = "@expiryId",ParamValue = expiryId , DBType=DbType.Int32},
                                new QueryParamObj{ParamName = "@companyId",ParamValue = companyId,DBType=DbType.Int32}
                            };

            var data = ExecuteStoreProcedure("Document_Expiry_EmployeeList", param, AttendanceMapper.ToExpiryReports);

            return data;
        }
    }
}
