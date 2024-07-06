using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.DataAccess
{
    public class LeavePolicyDataAccess : BaseDatabaseHandler, ILeavePolicy
    {
        public ResponseModel Delete(int id)
        {
            string err = string.Empty;
            string sql = @"DELETE FROM LeavePolicy WHERE Id = @id ";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32},

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            var response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }

        public List<LeavePolicyModel> GetAll()
        {
            const string sql = @"SELECT C.* FROM LeavePolicy C";
            return ExecuteDBQuery(sql, null, LeavePolicyMapper.ToModel);
        }

        public ResponseModel Save(LeavePolicyModel model)
        {
            var err = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamName = "@Id", ParamValue = model.Id,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@PolicyCode", ParamValue =model.PolicyCode},
                    new QueryParamObj { ParamName = "@Description", ParamValue =string.IsNullOrEmpty(model.Description)?null:model.Description},
                    new QueryParamObj { ParamName = "@CreatedAt", ParamValue =DateTime.UtcNow,DBType = DbType.DateTime},
                    new QueryParamObj { ParamName = "@LeaveDays", ParamValue =model.LeaveDays},
                };

            const string sql = @"IF @Id=0
                               BEGIN
                                INSERT INTO LeavePolicy(PolicyCode,Description,LeaveDays,CreatedDate)
                                VALUES(@PolicyCode,@Description,@LeaveDays,@CreatedAt)
                               END
                               ELSE
                               BEGIN
                                UPDATE LeavePolicy SET PolicyCode=@PolicyCode,Description=@Description,LeaveDays=@LeaveDays
                                WHERE Id=@Id
                               END";
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = err };
        }
    }

    public static class LeavePolicyMapper
    {
        public static List<LeavePolicyModel> ToModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<LeavePolicyModel>();

            while (readers.Read())
            {
                var model = new LeavePolicyModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    PolicyCode = Convert.ToString(readers["PolicyCode"]),
                    Description = Convert.ToString(readers["Description"]),
                    LeaveDays = Convert.IsDBNull(readers["LeaveDays"]) ? (int?)null : Convert.ToInt32(readers["LeaveDays"]),
                };

                models.Add(model);
            }

            return models;
        }
    }
}
