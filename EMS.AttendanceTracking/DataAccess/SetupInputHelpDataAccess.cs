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
    public class SetupInputHelpDataAccess:BaseDatabaseHandler,ISetupInputHelp
	{
        public ResponseModel Save(SetupInputHelpModel mInputHelp)
        {
            var err = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamName = "@Id", ParamValue = mInputHelp.Id,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@InputHelpTypeId", ParamValue =mInputHelp.InputHelpTypeId,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@Name", ParamValue =string.IsNullOrEmpty(mInputHelp.Name)?null:mInputHelp.Name},
                    new QueryParamObj { ParamName = "@IsActive", ParamValue =mInputHelp.IsActive,DBType = DbType.Boolean},
                    new QueryParamObj { ParamName = "@CreatedAt", ParamValue =DateTime.UtcNow,DBType = DbType.DateTime}
                };

            const string sql = @"IF @Id=0
                               BEGIN
                                INSERT INTO InputHelp(InputHelpTypeId,Name,IsActive,CreatedAt)
                                VALUES(@InputHelpTypeId,@Name,@IsActive,@CreatedAt)
                               END
                               ELSE
                               BEGIN
                                UPDATE InputHelp SET Name=@Name,InputHelpTypeId=@InputHelpTypeId,
                                IsActive=@IsActive
                                WHERE Id=@Id
                               END";
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = err };
        }

        public List<SetupInputHelpModel> GetAll()
		{
           
            const string sql = @"SELECT C.* FROM InputHelp C";
            return ExecuteDBQuery(sql, null, SetupInputHelpMapper.ToSetupInputHelpModel);

        }

        public ResponseModel DeleteInputHelp(int id)
        {
            var response = new ResponseModel();
            string err = string.Empty;
            string sql = @"DELETE FROM InputHelp WHERE Id = @id ";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32},

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }

        public ResponseModel UpdateQrCode(EmployeeExportModel qrCodeModel)
        {
            string err = string.Empty;
            var sql = @"UPDATE Employee SET QrCodeNo=@qrcode WHERE Id=@id";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =qrCodeModel.Id ,DBType = DbType.Int64},
                    new QueryParamObj { ParamName = "@qrcode", ParamValue =qrCodeModel.QrCodeNo}
                };
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "QR code updated sucessfully" : "Problem in Update" };
        }
    }
    public static class SetupInputHelpMapper
    {
        public static List<SetupInputHelpModel> ToSetupInputHelpModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<SetupInputHelpModel>();

            while (readers.Read())
            {
                var model = new SetupInputHelpModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    Name = Convert.ToString(readers["Name"]),
                    InputHelpTypeId = Convert.ToInt32(readers["InputHelpTypeId"]),
                    IsActive = Convert.IsDBNull(readers["IsActive"]) ? false : Convert.ToBoolean(readers["IsActive"]),
                };

                models.Add(model);
            }

            return models;
        }
    }
}
