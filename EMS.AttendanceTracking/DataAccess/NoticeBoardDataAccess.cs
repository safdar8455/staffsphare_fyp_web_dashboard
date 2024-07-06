using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.DataAccess
{
    public class NoticeBoardDataAccess : BaseDatabaseHandler, INoticeBoard
    {
        public ResponseModel Delete(string id)
        {
            string err = string.Empty;
            string sql = @"DELETE FROM NoticeBoard WHERE Id = @id ";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.String},

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            var response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }
        public ResponseModel Save(NoticeBoardModel model)
        {
            var err = string.Empty;
            var queryParamList = new QueryParamList
            {
                new QueryParamObj{ParamName="@Id",ParamValue=string.IsNullOrEmpty(model.Id)? Guid.NewGuid().ToString():model.Id},
                new QueryParamObj{ParamName="@CompanyId",ParamValue=model.CompanyId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@Details", ParamValue =string.IsNullOrEmpty(model.Details)?null:model.Details},
                new QueryParamObj { ParamName = "@PostingDate", ParamValue =DateTime.UtcNow,DBType = DbType.DateTime},
                new QueryParamObj { ParamName = "@CreateDate", ParamValue =DateTime.UtcNow,DBType = DbType.DateTime},
                new QueryParamObj { ParamName = "@ImageFileName", ParamValue =model.ImageFileName},
                new QueryParamObj { ParamName = "@CreatedBy", ParamValue =model.CreatedBy}
            };
            const string sql = @"IF Not Exists(select * From NoticeBoard Where Id=@id)
                               BEGIN
                                INSERT INTO NoticeBoard(Id,Details,PostingDate,ImageFileName,CreateDate,CreatedBy,CompanyId)
                                VALUES(@Id,@Details,@PostingDate,@ImageFileName,@CreateDate,@CreatedBy,@CompanyId)
                               END
                               ELSE
                               BEGIN
                                UPDATE NoticeBoard SET Details=@Details,ImageFileName=@ImageFileName
                                WHERE Id=@Id
                               END";
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = err };
            
        }

       public List<NoticeBoardModel> GetAll(int companyId)
        {
            const string sql = @"SELECT N.* FROM NoticeBoard N where (isnull(@companyId,0)=0 or N.companyId=@companyId)";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@companyId", ParamValue =companyId ,DBType = DbType.Int32},

                };
            return ExecuteDBQuery(sql, queryParamList, NoticeBoardMapper.ToModel);

        }

        public NoticeBoardModel Get(string id)
        {
            const string sql = @"SELECT N.* FROM NoticeBoard N where N.Id=@id";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id}

                };
            var data= ExecuteDBQuery(sql, queryParamList, NoticeBoardMapper.ToModel);
            return data != null && data.Count > 0 ? data.FirstOrDefault() : new NoticeBoardModel();
        }

        public ResponseModel UploadImageFile(NoticeBoardModel model)
        {
            string err = string.Empty;
            var sql = @"UPDATE NoticeBoard SET
	                        ImageFileName=@UploadedFileName
	                        WHERE Id=@Id ";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@Id", ParamValue =model.Id },
                    new QueryParamObj { ParamName = "@UploadedFileName", ParamValue =model.AttachedDocument.UploadedFileName}
                };
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Updated sucessfully" : "Problem in Update" };
        }
    }

    public static class NoticeBoardMapper
    {
        public static List<NoticeBoardModel> ToModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<NoticeBoardModel>();

            while (readers.Read())
            {
                var model = new NoticeBoardModel
                {
                    Id = Convert.ToString(readers["Id"]),
                    CompanyId = Convert.IsDBNull(readers["CompanyId"]) ? (int?)null : Convert.ToInt32(readers["CompanyId"]),
                    Details = Convert.IsDBNull(readers["Details"]) ? string.Empty : Convert.ToString(readers["Details"]),
                    PostingDate = Convert.IsDBNull(readers["PostingDate"]) ?(DateTime?)null: Convert.ToDateTime(readers["PostingDate"]),
                    CreateDate = Convert.IsDBNull(readers["CreateDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["CreateDate"]),
                    ImageFileName = Convert.IsDBNull(readers["ImageFileName"]) ? string.Empty : Convert.ToString(readers["ImageFileName"]),
                    CreatedBy = Convert.IsDBNull(readers["CreatedBy"]) ? string.Empty : Convert.ToString(readers["CreatedBy"]),
                };
                model.ImagePath = Constants.LocalFilePath + model.ImageFileName;
                models.Add(model);
            }

            return models;
        }
    }
}
