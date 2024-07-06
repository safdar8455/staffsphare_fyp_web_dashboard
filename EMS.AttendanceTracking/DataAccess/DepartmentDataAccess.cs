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
    public class DepartmentDataAccess : BaseDatabaseHandler, IDepartment
    {
        public ResponseModel Delete(int id)
        {
            string err = string.Empty;
            string sql = @"DELETE FROM Department WHERE Id = @id ";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32},

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            var response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }

        public List<DepartmentModel> GetAll()
        {
            const string sql = @"SELECT D.*,C.CompanyName
							FROM Department D
							LEFT JOIN Company C ON C.Id=D.CompanyId";
            return ExecuteDBQuery(sql, null, DepartmentMapper.ToModel);
        }

        public ResponseModel Save(DepartmentModel model)
        {
            var err = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamName = "@Id", ParamValue = model.Id,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@CompanyId", ParamValue = model.CompanyId,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@Code", ParamValue =model.Code},
                    new QueryParamObj { ParamName = "@DepartmentName", ParamValue =model.DepartmentName},
                    new QueryParamObj { ParamName = "@CreatedAt", ParamValue =DateTime.UtcNow,DBType = DbType.DateTime}
                };

            const string sql = @"IF @Id=0
                               BEGIN
                                INSERT INTO Department(Code,DepartmentName,CreatedDate,CompanyId)
                                VALUES(@Code,@DepartmentName,@CreatedAt,@CompanyId)
                               END
                               ELSE
                               BEGIN
                                UPDATE Department SET Code=@Code,DepartmentName=@DepartmentName,
                                CompanyId=@CompanyId
                                WHERE Id=@Id
                               END";
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = err };
        }
    }

    public static class DepartmentMapper
    {
        public static List<DepartmentModel> ToModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<DepartmentModel>();

            while (readers.Read())
            {
                var model = new DepartmentModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    Code = Convert.ToString(readers["Code"]),
                    DepartmentName = Convert.ToString(readers["DepartmentName"]),
                    CompanyId = Convert.IsDBNull(readers["CompanyId"]) ? (int?)null : Convert.ToInt32(readers["CompanyId"]),
                    CompanyName = Convert.IsDBNull(readers["CompanyName"]) ? string.Empty : Convert.ToString(readers["CompanyName"])
                };

                models.Add(model);
            }

            return models;
        }
    }
}
