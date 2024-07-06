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
    public class DesignationDataAccess : BaseDatabaseHandler, IDesignation
    {
        public ResponseModel Delete(int id)
        {
            string err = string.Empty;
            string sql = @"DELETE FROM Designation WHERE Id = @id ";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32},

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            var response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }

        public List<DesignationModel> GetAll(int companyId)
        {
            const string sql = @"SELECT C.* FROM Designation C where c.CompanyId=@companyId";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@companyId", ParamValue =companyId ,DBType = DbType.Int32}

                };
            return ExecuteDBQuery(sql, queryParamList, DesignationMapper.ToModel);
        }

        public ResponseModel Save(DesignationModel model)
        {
            var err = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamName = "@Id", ParamValue = model.Id,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@Code", ParamValue =model.Code},
                    new QueryParamObj { ParamName = "@Name", ParamValue =string.IsNullOrEmpty(model.Name)?null:model.Name},
                    new QueryParamObj { ParamName = "@CreatedAt", ParamValue =DateTime.UtcNow,DBType = DbType.DateTime},
                    new QueryParamObj { ParamName = "@CompanyId", ParamValue =model.CompanyId,DBType = DbType.Int32}
                };

            const string sql = @"IF @Id=0
                               BEGIN
                                INSERT INTO Designation(Code,Name,CreatedDate,CompanyId)
                                VALUES(@Code,@Name,@CreatedAt,@CompanyId)
                               END
                               ELSE
                               BEGIN
                                UPDATE Designation SET Code=@Code,Name=@Name
                                WHERE Id=@Id
                               END";
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = err };
        }
    }

    public static class DesignationMapper
    {
        public static List<DesignationModel> ToModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<DesignationModel>();

            while (readers.Read())
            {
                var model = new DesignationModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    Code = Convert.ToString(readers["Code"]),
                    Name = Convert.ToString(readers["Name"])
                };

                models.Add(model);
            }

            return models;
        }
    }
}
