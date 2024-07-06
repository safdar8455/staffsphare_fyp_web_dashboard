using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Models;
using System.Linq;

namespace Ems.AttendanceTracking.DataAccess
{
    public class HolidayDataAccess : BaseDatabaseHandler, IHoliday
    {
        public ResponseModel Delete(int id)
        {
            string err = string.Empty;
            string sql = @"DELETE FROM CompanyHoliday WHERE Id = @id ";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32},

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            var response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }

        public List<HolidayModel> GetAll(int? cId)
        {
            const string sql = @"SELECT C.* FROM CompanyHoliday C where c.CompanyId=@cId";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@cId", ParamValue =cId ,DBType = DbType.Int32}

                };
            return ExecuteDBQuery(sql, queryParamList, HolidayMapper.ToModel);
        }

        public HolidayModel GetById(int id)
        {
            const string sql = @"SELECT C.* FROM CompanyHoliday C where c.Id=@id";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32}

                };
            var data= ExecuteDBQuery(sql, queryParamList, HolidayMapper.ToModel);
            return data.FirstOrDefault();
        }

        public ResponseModel Save(HolidayModel model)
        {
            var err = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamName = "@Id", ParamValue = model.Id,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@Name", ParamValue =model.Name},
                    new QueryParamObj { ParamName = "@HolidayDate", ParamValue =string.IsNullOrEmpty(model.HolidayDateStr)? DateTime.Now:Convert.ToDateTime(model.HolidayDateStr),DBType=DbType.DateTime},
                    new QueryParamObj { ParamName = "@CreatedAt", ParamValue =DateTime.UtcNow,DBType = DbType.DateTime},
                    new QueryParamObj { ParamName = "@TypeId", ParamValue =model.TypeId},
                    new QueryParamObj { ParamName = "@CompanyId", ParamValue =model.CompanyId}
                };

            const string sql = @"IF @Id=0
                               BEGIN
                                INSERT INTO CompanyHoliday(Name,HolidayDate,TypeId,CreatedDate,CompanyId)
                                VALUES(@Name,@HolidayDate,@TypeId,@CreatedAt,@CompanyId)
                               END
                               ELSE
                               BEGIN
                                UPDATE CompanyHoliday SET Name=@Name,HolidayDate=@HolidayDate,TypeId=@TypeId
                                WHERE Id=@Id
                               END";
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = err };
        }
    }

    public static class HolidayMapper
    {
        public static List<HolidayModel> ToModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<HolidayModel>();

            while (readers.Read())
            {
                var model = new HolidayModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    Name = Convert.ToString(readers["Name"]),
                    HolidayDate = Convert.IsDBNull(readers["HolidayDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["HolidayDate"]),
                    HolidayDateStr = Convert.IsDBNull(readers["HolidayDate"]) ?string.Empty : Convert.ToDateTime(readers["HolidayDate"]).ToString(Constants.DateFormat),
                    TypeId = Convert.IsDBNull(readers["TypeId"]) ? (int?)null : Convert.ToInt32(readers["TypeId"]),
                };

                models.Add(model);
            }

            return models;
        }
    }
}
