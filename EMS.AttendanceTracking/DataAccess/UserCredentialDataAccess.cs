using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;

namespace Ems.AttendanceTracking.DataAccess
{
    public class UserCredentialDataAccess : BaseDatabaseHandler, IUserCredential
    {
        public ResponseModel Save(UserCredentialModel model)
        {
            var errMessage = string.Empty;
            var pk = Guid.NewGuid().ToString();
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue = pk},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@FullName", ParamValue = model.FullName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Email", ParamValue =model.Email},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LoginID", ParamValue =model.LoginID},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ContactNo", ParamValue = model.ContactNo},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Password", ParamValue =model.Password},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserTypeId", ParamValue =model.UserTypeId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyName", ParamValue =string.IsNullOrEmpty(model.CompanyName)?null:model.CompanyName}
                };

            const string sql = @"INSERT INTO UserCredentials(Id,FullName,Email,ContactNo,Password,UserTypeId,IsActive,CreatedAt,LoginID)
                                VALUES(@Id,@FullName,@Email,@ContactNo,@Password,@UserTypeId,1,GETDATE(),@LoginID)
                                
                                  IF @CompanyName is not null
                                    BEGIN
                                    DECLARE @cId INT
                                     INSERT INTO ResourceTracker_Company(PortalUserId,CompanyName,PhoneNumber) VALUES(@Id,@CompanyName,@ContactNo)
                                     SET @cId=SCOPE_IDENTITY();
                                     INSERT INTO ResourceTracker_Department(CompanyId,DepartmentName) VALUES(@cId,'Trial Department')
                                        DECLARE @dId INT
                                        SET @dId=SCOPE_IDENTITY(); 
                                     INSERT INTO ResourceTracker_Designation(CompanyId,DesignationName) VALUES(@cId,'Trial Designation')
                                     INSERT INTO ResourceTracker_EmployeeUser(UserId,UserName,CompanyId,DepartmentId,PhoneNumber,IsAutoCheckPoint,IsActive)
                                     values(@Id,@FullName,@cId,@dId,@ContactNo,0,1)
                                    END";


            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage), Message = errMessage, ReturnCode = pk };

        }
        public List<UserCredentialModel> GetAllUser()
        {
            const string sql = @"SELECT U.*,C.CompanyName FROM UserCredentials U
                                LEFT JOIN Company C ON U.CompanyId=C.Id
                                where u.LoginID<>'superadmin'";
            var results = ExecuteDBQuery(sql, null, UserMapper.ToUserList);
            return results;
        }
        public List<UserCredentialModel> GetAllUser(int cId)
        {
            const string sql = @"SELECT U.*,C.CompanyName FROM UserCredentials U
                                LEFT JOIN Company C ON U.CompanyId=C.Id
                                where u.companyid=@cId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@cId", ParamValue = cId}
                };
            var results = ExecuteDBQuery(sql, queryParamList, UserMapper.ToUserList);
            return results;
        }
        public List<UserCredentialModel> GetAll(UserCredentialModel searchModel)
        {
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue = searchModel.Id},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UserTypeId", ParamValue = (searchModel.UserTypeId !=0?searchModel.UserTypeId:(int?)null),DBType = DbType.Int32}
                };
            return ExecuteStoreProcedure("UserCredential_GetAllUser", queryParamList, UserMapper.ToUserCredentialDetailsModel);
        }
        public ResponseModel Update(UserCredentialModel model)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue = model.Id},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@FullName", ParamValue = model.FullName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Email", ParamValue =model.Email},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ContactNo", ParamValue = model.ContactNo},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@IsActive", ParamValue =model.IsActive},
                };

            const string sql = @"UPDATE UserCredentials SET FullName=@FullName,Email=@Email,ContactNo=@ContactNo,
                            IsActive=@IsActive where Id=@Id";


            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage), Message = errMessage, ReturnCode = model.Id };

        }
        public UserCredentialModel Get(string username, string password)
        {

            const string sql = @"SELECT c.*,e.HasAccessQrCodeScan,B.CompanyName,B.Address,B.IsMultipleDevieAllow
                                FROM UserCredentials C 
                                LEFT JOIN Company B ON B.Id=C.CompanyId
                                left join Employee e on e.PortalUserId=c.Id
                                WHERE C.LoginID=@Username and C.Password=@Password AND C.IsActive=1";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Username", ParamValue = username},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Password", ParamValue = password}
                };
            var results = ExecuteDBQuery(sql, queryParamList, UserMapper.ToUserFullDetails);
            return results.Any() ? results.FirstOrDefault() : null;

        }

        public ResponseModel ChangePassword(string userInitial, string newPassword)
        {
            var err = string.Empty;

            const string sql = @"UPDATE UserCredentials set Password=@newPassword where LoginID=@userInitial";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@userInitial", ParamValue =userInitial},
                    new QueryParamObj { ParamName = "@newPassword", ParamValue =newPassword}
                };

            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }
        public ResponseModel updateDeviceIdentifier(DeviceInformation model)
        {
            var err = string.Empty;

            const string sql = @"UPDATE UserCredentials set UniqueDeviceIdentifier=@UniqueDeviceIdentifier,deviceName=@deviceName,brand=@brand,modelName=@modelName,osName=@osName,osVersion=@osVersion,osBuildId=@osBuildId,LastloginTime=@LastloginTime where Id=@userId";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@UniqueDeviceIdentifier", ParamValue =model.UniqueDeviceIdentifier},
                    new QueryParamObj { ParamName = "@deviceName", ParamValue =model.deviceName},
                    new QueryParamObj { ParamName = "@brand", ParamValue =model.brand},
                    new QueryParamObj { ParamName = "@modelName", ParamValue =model.modelName},
                    new QueryParamObj { ParamName = "@osName", ParamValue =model.osName},
                    new QueryParamObj { ParamName = "@osVersion", ParamValue =model.osVersion},
                    new QueryParamObj { ParamName = "@osBuildId", ParamValue =model.osBuildId},
                    new QueryParamObj { ParamName = "@userId", ParamValue =model.userId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@LastloginTime", ParamValue = DateTime.UtcNow,DBType=DbType.DateTime},
                };

            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }
        
        public ResponseModel ChangePasswordByPk(string userKey, string newPassword)
        {
            var err = string.Empty;

            const string sql = @"UPDATE UserCredentials set Password=@newPassword where Id=@userKey";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@userKey", ParamValue =userKey},
                    new QueryParamObj { ParamName = "@newPassword", ParamValue =newPassword}
                };

            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }

        public UserCredentialModel GetProfileDetails(string userId)
        {
            const string sql = @"SELECT c.*,e.HasAccessQrCodeScan,B.CompanyName,B.Address,B.IsMultipleDevieAllow
                                FROM UserCredentials C
                                LEFT JOIN Company B ON B.Id=C.CompanyId
                                left join Employee e on e.PortalUserId=c.Id
                                WHERE c.Id=@userId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue = userId}
                };
            var results = ExecuteDBQuery(sql, queryParamList, UserMapper.ToUserFullDetails);
            return results.Any() ? results.FirstOrDefault() : null;
        }

        public UserCredentialModel GetByLoginID(string loginID)
        {
            const string sql = @"SELECT c.*,e.HasAccessQrCodeScan,B.CompanyName,B.Address
                                FROM UserCredentials C
                                LEFT JOIN Company B ON B.Id=C.CompanyId
                                left join Employee e on e.PortalUserId=c.Id
                                WHERE c.LoginID=@loginID";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@loginID", ParamValue = loginID}
                };
            var results = ExecuteDBQuery(sql, queryParamList, UserMapper.ToUserCredentialDetailsModel);
            return results.Any() ? results.FirstOrDefault() : null;
        }

        public UserCredentialModel GetByLoginID(string loginID,UserType uType)
        {
            const string sql = @"SELECT c.*,e.HasAccessQrCodeScan,B.CompanyName,B.Address
                                FROM UserCredentials C
                                LEFT JOIN Company B ON B.Id=C.CompanyId
                                left join Employee e on e.PortalUserId=c.Id
                                WHERE c.LoginID=@loginID AND C.UserTypeId=@uType AND C.IsActive=1";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@loginID", ParamValue = loginID},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@uType", ParamValue = (int)uType}
                };
            var results = ExecuteDBQuery(sql, queryParamList, UserMapper.ToUserCredentialDetailsModel);
            return results.Any() ? results.FirstOrDefault() : null;
        }

        public UserCredentialModel GetByLoginID(string loginID,string password ,UserType uType)
        {
            const string sql = @"SELECT c.*,e.HasAccessQrCodeScan,B.CompanyName,B.Address
                                FROM UserCredentials C
                                LEFT JOIN Company B ON B.Id=C.CompanyId
                                left join Employee e on e.PortalUserId=c.Id
                                WHERE c.LoginID=@loginID AND C.Password=@password AND C.UserTypeId=@uType";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@loginID", ParamValue = loginID},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@password", ParamValue = password},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@uType", ParamValue = (int)uType}
                };
            var results = ExecuteDBQuery(sql, queryParamList, UserMapper.ToUserCredentialDetailsModel);
            return results.Any() ? results.FirstOrDefault() : null;
        }

  

        public UserCredentialModel GetUserFullInfo(string userId)
        {
            const string sql = @"SELECT c.* FROM UserCredentials c WHERE c.Id=@userId and c.IsActive=1";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue = userId}
                };
            var results = ExecuteDBQuery(sql, queryParamList, UserMapper.ToUserFullInfo);
            return results.Any() ? results.FirstOrDefault() : null;
        }

        public ResponseModel RemoveDevice(string uId)
        {
            var err = string.Empty;

            const string sql = @"UPDATE UserCredentials set 
                                UniqueDeviceIdentifier=NULL,deviceName=NULL,brand=NULL,modelName=NULL,osName=NULL,osVersion=NULL,osBuildId=NULL
                                where Id=@uId";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@uId", ParamValue =uId}
                };

            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }

        public ResponseModel ResetUserPass(string uId)
        {
            var err = string.Empty;

            const string sql = @"UPDATE UserCredentials set Password=@pass where Id=@uId";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@uId", ParamValue =uId},
                     new QueryParamObj { ParamName = "@pass", ParamValue =CryptographyHelper.CreateMD5Hash(Constants.ResetPassword)}
                };

            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err) };
        }

    }
}