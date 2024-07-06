using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.DataAccess
{
    public class CompanyDataAccess : BaseDatabaseHandler, ICompany
    {
        public List<Company> GetCompanyList()
        {
            string err = string.Empty;
            string sql = @"select c.*,e.EmployeeName
                            from Company c
                            left join Employee e on e.EmployeeCode=c.HrDirectorCode";
            var results = ExecuteDBQuery(sql, null, CompanyMapper.ToCompanyModel);
            return results;
        }

        public ResponseModel Create(Company model)
        {
            var err = string.Empty;

            var queryParamList = new QueryParamList
                    {
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =model.Id},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@HrDirectorCode", ParamValue =model.HrDirectorCode},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyName", ParamValue =model.CompanyName},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Address", ParamValue =model.Address},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@PhoneNumber", ParamValue =model.PhoneNumber},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedById", ParamValue =model.CreatedById},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsActive", ParamValue =model.IsActive},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsMultipleDevieAllow", ParamValue =model.IsMultipleDevieAllow},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminName", ParamValue =model.CompanyAdminName},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminEmail", ParamValue =model.CompanyAdminEmail},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminLoginID", ParamValue =model.CompanyAdminLoginID},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminPassword", ParamValue =model.CompanyAdminPassword},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserKey", ParamValue =Guid.NewGuid().ToString()},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedDate", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                    };


            const string sql = @"IF NOT EXISTS(SELECT TOP 1 * FROM Company C WHERE C.Id=@Id)
                                BEGIN
                                DECLARE @cId INT
                                INSERT INTO Company(CompanyName,HrDirectorCode,Address,PhoneNumber,CreatedDate,CreatedById,IsActive,CompanyAdminName,CompanyAdminEmail,CompanyAdminLoginID,IsMultipleDevieAllow) 
                                VALUES(@CompanyName,@HrDirectorCode,@Address,@PhoneNumber,@CreatedDate,@CreatedById,1,@CompanyAdminName,@CompanyAdminEmail,@CompanyAdminLoginID,@IsMultipleDevieAllow)
                                SET @cId=SCOPE_IDENTITY()
                                INSERT INTO UserCredentials(Id,FullName,Email,LoginID,Password,UserTypeId,IsActive,CreatedAt,CompanyId)
                                VALUES(@UserKey,@CompanyAdminName,@CompanyAdminEmail,@CompanyAdminLoginID,@CompanyAdminPassword,2,1,@CreatedDate,@cId)
                                END
                                ELSE
                                BEGIN
                                UPDATE Company  SET CompanyName=@CompanyName, Address =@Address ,PhoneNumber = @PhoneNumber,CompanyAdminName=@CompanyAdminName,CompanyAdminEmail=@CompanyAdminEmail,IsActive=@IsActive,IsMultipleDevieAllow=@IsMultipleDevieAllow WHERE Id=@Id
                                END";
            DBExecCommandEx(sql, queryParamList, ref err);

            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }
        public ResponseModel Update(Company model)
        {
            var err = string.Empty;
            const string sql = @"UPDATE Company  SET HrDirectorCode=@HrDirectorCode,CompanyName=@CompanyName, Address =@Address ,PhoneNumber = @PhoneNumber,CompanyAdminName=@CompanyAdminName,CompanyAdminEmail=@CompanyAdminEmail,IsActive=@IsActive,IsMultipleDevieAllow=@IsMultipleDevieAllow WHERE Id=@Id";
            var queryParamList = new QueryParamList
                           {
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =model.Id},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@HrDirectorCode", ParamValue =model.HrDirectorCode},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyName", ParamValue =model.CompanyName},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Address", ParamValue =model.Address},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@PhoneNumber", ParamValue =model.PhoneNumber},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminName", ParamValue =model.CompanyAdminName},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminEmail", ParamValue =model.CompanyAdminEmail},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsActive", ParamValue =model.IsActive},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsMultipleDevieAllow", ParamValue =model.IsMultipleDevieAllow},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedDate", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},

                            };
            DBExecCommandEx(sql, queryParamList, ref err);

            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }

        public ResponseModel UpdateLogo(int id, string fileId, string fileName)
        {
            var err = string.Empty;
            const string sql = @"UPDATE Company  SET ImageFileName=@ImageFileName, ImageFileId =@ImageFileId WHERE Id=@Id";
            var queryParamList = new QueryParamList
                           {
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =id},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ImageFileName", ParamValue =fileName},
                                new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ImageFileId", ParamValue =fileId}

                            };
            DBExecCommandEx(sql, queryParamList, ref err);

            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }


        public ResponseModel Delete(int id)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                    {
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@id", ParamValue =id},
                    };
            const string sql = @"DELETE FROM UserCredentials WHERE CompanyId=@id
                                delete from Company where Id=@id";
            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }

        public ResponseModel AddOrUpdateCompany(Company model)
        {
            var returnId = (int?)null;
            var db = GetSQLDatabase();
            var message = string.Empty;
            using (DbConnection conn = db.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    returnId = SaveCompany(model, db, trans);
                    if (returnId > 0)
                    {
                        if (model.AttachedDocumentList != null && model.AttachedDocumentList.Count > 0)
                        {
                            foreach (var att in model.AttachedDocumentList)
                            {
                                SaveCompanyAttachment(att, returnId.Value, db, trans, model.CreatedById);
                            }
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    message = ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            var success = string.IsNullOrEmpty(message);
            return new ResponseModel { Success = success };
        }
        private int SaveCompany(Company model, Database db, DbTransaction trans)
        {
            DbCommand templateCommand = db.GetStoredProcCommand("Company_Save");

            var queryParamList = new QueryParamList
                    {
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =model.Id,DBType = DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@HrDirectorCode", ParamValue =model.HrDirectorCode},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyName", ParamValue =model.CompanyName},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Address", ParamValue =model.Address},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@PhoneNumber", ParamValue =model.PhoneNumber},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedById", ParamValue =model.CreatedById},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsActive", ParamValue =model.IsActive},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsMultipleDevieAllow", ParamValue =model.IsMultipleDevieAllow},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminName", ParamValue =model.CompanyAdminName},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminEmail", ParamValue =model.CompanyAdminEmail},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminLoginID", ParamValue =model.CompanyAdminLoginID},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyAdminPassword", ParamValue =model.CompanyAdminPassword},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserKey", ParamValue =Guid.NewGuid().ToString()},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedDate", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyRegistrationNumber", ParamValue =model.CompanyRegistrationNumber},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyRegistrationExpiryDate", ParamValue =model.CompanyRegistrationExpiryDate,DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyRegistrationExpiresInDays", ParamValue =model.CompanyRegistrationExpiresInDays,DBType=DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@EstablishmentCardNumber", ParamValue =model.EstablishmentCardNumber},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@EstablishmentCardExpiryDate", ParamValue =model.EstablishmentCardExpiryDate,DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@EstablishmentCardExpiresInDays", ParamValue =model.EstablishmentCardExpiresInDays,DBType=DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TradeLicenseNumber", ParamValue =model.TradeLicenseNumber},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TradeLicenseExpiryDate", ParamValue =model.TradeLicenseExpiryDate,DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TradeLicenseExpiresInDays", ParamValue =model.TradeLicenseExpiresInDays,DBType=DbType.Int32},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@OthersExpiryDate", ParamValue =model.OthersExpiryDate,DBType=DbType.DateTime},
                        new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@OthersExpiresInDays", ParamValue =model.OthersExpiresInDays,DBType=DbType.Int32},
                    };
            var identityParam = new QueryParamObj
            {
                ParamName = "@ReturnId",
                ParamDirection = ParameterDirection.Output,
                DBType = DbType.Int32
            };
            queryParamList.Add(identityParam);

            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
            identityParam.ParamValue = db.GetParameterValue(templateCommand, identityParam.ParamName);
            return Convert.ToInt32(identityParam.ParamValue);
        }
        private void SaveCompanyAttachment(AttachmentModel model, int companyId, Database db, DbTransaction trans, string createdById)
        {
            string err = string.Empty;
            string sql = @"INSERT INTO CompanyAttachements(CompanyId, BlobName, [FileName], CreatedAt, CreatedById, AttachmentTypeId)
					        VALUES(@CompanyId, @BlobName, @FileName, @CreatedAt, @CreatedById, @AttachmentTypeId)";
            var queryParamList = new QueryParamList
                    {
                       new QueryParamObj { ParamName = "@CompanyId", ParamValue = companyId,DBType = DbType.Int32},
                       new QueryParamObj { ParamName = "@BlobName", ParamValue = model.BlobName},
                       new QueryParamObj { ParamName = "@FileName", ParamValue = model.FileName},
                       new QueryParamObj { ParamName = "@CreatedById", ParamValue = createdById},
                       new QueryParamObj { ParamName = "@CreatedAt", ParamValue = DateTime.UtcNow,DBType = DbType.DateTime},
                       new QueryParamObj { ParamName = "@AttachmentTypeId", ParamValue = model.AttachmentTypeId,DBType = DbType.Int32},
                    };
            DBExecCommandExTran(sql, queryParamList, trans, db, ref err);
        }
        public ResponseModel DeleteCompanyAttachments(int id)
        {
            string err = string.Empty;
            var sql = @"DELETE FROM CompanyAttachements WHERE Id=@id";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32}
                };
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete successfully" : "Problem in Delete" };
        }
        public List<AttachmentModel> GetCompanyAttachments(int id)
        {
            const string sql = @"SELECT Id, CompanyId,[FileName],BlobName, AttachmentTypeId
                                FROM CompanyAttachements
                                WHERE CompanyId= @CompanyId";
            var paramList = new QueryParamList {
                new QueryParamObj { ParamName = "@CompanyId", ParamValue = id, DBType=DbType.Int32 },
            };

            var list = ExecuteDBQuery(sql, paramList, CompanyMapper.ToAttachFilesModel);
            if (list == null)
                list = new List<AttachmentModel>();

            return list;
        }
    }
}
