using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;


namespace Ems.AttendanceTracking.DataAccess
{
    public class LeaveDataAccess : BaseDatabaseHandler, ILeave
    {
        public List<LeaveModel> GetLeaveByCompanyId(int companyId)
        {
            string err = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue =companyId},
                };
            string sql = @"SELECT la.* ,eu.PortalUserId,uc.FullName as EmployeeName,AP.FullName ApprovedBy,P.Description LeaveType 
                            from LeaveApplication as la
                            Left JOIN Employee eu on eu.id= la.EmployeeId
                            Left JOIN UserCredentials uc on uc.id= eu.PortalUserId
                            LEFT JOIN UserCredentials AP ON LA.ApprovedById=AP.Id
                            LEFT JOIN LeavePolicy P ON LA.LeaveTypeId=P.Id
                            where (isnull(@companyId,0)=0 or la.CompanyId=@companyId) order by la.Id desc";
            var results = ExecuteDBQuery(sql, queryParamList, LeaveMapper.ToEmployeeLeaveMapperModel);
            return results.Any() ? results : new List<LeaveModel>();
        }
        public LeaveModel GetLeaveById(int Id)
        {
            string err = string.Empty;
            string sql = @"SELECT la.* ,eu.PortalUserId,eu.EmployeeName,AP.FullName ApprovedBy,P.Description LeaveType  from LeaveApplication as la
                            Left JOIN Employee eu on eu.id= la.EmployeeId
                            Left JOIN UserCredentials uc on uc.id= eu.PortalUserId
                            LEFT JOIN UserCredentials AP ON LA.ApprovedById=AP.Id
                            LEFT JOIN LeavePolicy P ON LA.LeaveTypeId=P.Id
                            where la.Id=@Id order by la.Id desc";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =Id , DBType = DbType.Int32},
                };
            var results = ExecuteDBQuery(sql, queryParamList, LeaveMapper.ToEmployeeLeaveMapperModel);
            return results.FirstOrDefault();
        }
        public List<LeaveModel> GetUserLeaves(string userId)
        {
            string err = string.Empty;
            string sql = @"SELECT la.* ,eu.PortalUserId,eu.EmployeeName,AP.FullName ApprovedBy,P.Description LeaveType  from LeaveApplication as la
                            Left JOIN Employee eu on eu.id= la.EmployeeId
                            Left JOIN UserCredentials uc on uc.id= eu.PortalUserId
                            LEFT JOIN UserCredentials AP ON LA.ApprovedById=AP.Id
                            LEFT JOIN LeavePolicy P ON LA.LeaveTypeId=P.Id
                            where  uc.id =@userId  order by la.Id desc";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue =userId},
                };
            var results = ExecuteDBQuery(sql, queryParamList, LeaveMapper.ToEmployeeLeaveMapperModel);
            return results.Any() ? results : new List<LeaveModel>();
        }
        public List<LeaveModel> GetUserPendingLeaves(string userId)
        {
            string err = string.Empty;
            string sql = @"SELECT la.* ,eu.PortalUserId,eu.EmployeeName,AP.FullName ApprovedBy,P.Description LeaveType  from LeaveApplication as la
                            Left JOIN Employee eu on eu.id= la.EmployeeId
                            Left JOIN UserCredentials uc on uc.id= eu.PortalUserId
                            LEFT JOIN UserCredentials AP ON LA.ApprovedById=AP.Id
                            LEFT JOIN LeavePolicy P ON LA.LeaveTypeId=P.Id
                            where  la.NextApproverId =@userId  order by la.Id desc";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue =userId},
                };
            var results = ExecuteDBQuery(sql, queryParamList, LeaveMapper.ToEmployeeLeaveMapperModel);
            return results.Any() ? results : new List<LeaveModel>();
        }
        public ResponseModel CreateEmployeeLeave(LeaveModel model)
        {
            var errMessage = string.Empty;
            Database db = GetSQLDatabase();
            var returnId = -1;
            using (DbConnection conn = db.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    if (model.Id == 0)
                    {
                        returnId = SaveEmployeeLeave(model, db, trans);
                    }
                    else
                    {
                        returnId = UpdateEmployeeLeave(model, db, trans);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    errMessage = ex.Message;
                }
            }
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage), Message = errMessage };
        }
        public int UpdateEmployeeLeave(LeaveModel model, Database db, DbTransaction trans)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                    {
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue = model.Id},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@FromDate", ParamValue = model.FromDate.ToString(Constants.ServerDateTimeFormat), DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@StatusId", ParamValue = model.StatusId,DBType = DbType.Int32},

                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ToDate", ParamValue = model.ToDate.ToString(Constants.ServerDateTimeFormat), DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LeaveTypeId", ParamValue =model.LeaveTypeId, DBType = DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LeaveReason", ParamValue = model.LeaveReason},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsApproved", ParamValue = model.IsApproved, DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsCorrection", ParamValue = model.IsCorrection, DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsRejected", ParamValue =model.IsRejected, DBType=DbType.Boolean},
                    };
            const string sql = @"Update LeaveApplication set FromDate=@FromDate,ToDate=@ToDate,LeaveTypeId=@LeaveTypeId,LeaveReason=@LeaveReason,IsApproved=@IsApproved,IsCorrection=@IsCorrection,StatusId=@StatusId,IsRejected=@IsRejected where Id=@Id";
            return DBExecCommandExTran(sql, queryParamList, trans, db, ref errMessage);
        }
        public int SaveEmployeeLeave(LeaveModel model, Database db, DbTransaction trans)
        {
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_Leave_Save");
            var queryParamList = new QueryParamList
                    {
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyId", ParamValue = model.CompanyId,DBType = DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@StatusId", ParamValue = model.StatusId,DBType = DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue = model.UserId},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@FromDate", ParamValue = model.FromDate.ToString(Constants.ServerDateTimeFormat), DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ToDate", ParamValue = model.ToDate.ToString(Constants.ServerDateTimeFormat), DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsHalfDay", ParamValue =model.IsHalfDay, DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LeaveTypeId", ParamValue =model.LeaveTypeId, DBType = DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LeaveReason", ParamValue = model.LeaveReason},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedAt", ParamValue =  Convert.ToDateTime(model.CreatedAt),DBType = DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsApproved", ParamValue = model.IsApproved, DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsCorrection", ParamValue = model.IsCorrection, DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsRejected", ParamValue =model.IsRejected, DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@RejectReason", ParamValue =model.RejectReason},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ApprovedById", ParamValue =model.ApprovedById},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ApprovedAt", ParamValue =model.ApprovedAt, DBType = DbType.DateTime},
                    };
            return DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
        }

        public ResponseModel Approved(int id, string userId)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =id},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ApprovedById", ParamValue =userId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ApprovedAt", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                };

            const string sql = @"UPDATE LeaveApplication SET IsApproved=1,ApprovedById=@ApprovedById,ApprovedAt=@ApprovedAt WHERE Id=@Id";
            DBExecCommandEx(sql, queryParamList, ref errMessage);
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }
        
        public ResponseModel Rejected(int id)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =id},
                };

            const string sql = @"UPDATE LeaveApplication SET IsRejected=1 WHERE Id=@Id";
            DBExecCommandEx(sql, queryParamList, ref errMessage);
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }
        public ResponseModel Correction(int id)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =id},
                };

            const string sql = @"UPDATE LeaveApplication SET IsCorrection=1 WHERE Id=@Id";
            DBExecCommandEx(sql, queryParamList, ref errMessage);
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }

        public ResponseModel ApproveOrRejectLeave(LeaveApproveModel model)
        {
            var errMessage = string.Empty;
            Database db = GetSQLDatabase();
            using (DbConnection conn = db.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    ApproveOrRejectEmployeeLeave(model, db, trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    errMessage = ex.Message;
                }
            }
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage), Message = errMessage };
        }

        private void ApproveOrRejectEmployeeLeave(LeaveApproveModel model, Database db, DbTransaction trans)
        {
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_Leave_Approve");
            var queryParamList = new QueryParamList
                    {
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@RequestNo", ParamValue = model.RequestNo},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Approved", ParamValue = model.Approved,DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Rejected", ParamValue = model.Rejected,DBType=DbType.Boolean},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LeaveApproved", ParamValue = (int)LeaveStatus.Approved,DBType=DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LeaveRejected", ParamValue = (int)LeaveStatus.Rejected,DBType=DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@SerialNo", ParamValue = model.SerialNo,DBType=DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@RejectReason", ParamValue = model.RejectReason},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ApproverId", ParamValue = model.ApproverId},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UpdatedById", ParamValue = model.UpdatedById},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UpdatedAt", ParamValue = model.UpdatedAt,DBType=DbType.DateTime}
                    };
            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
        }
    }

}
