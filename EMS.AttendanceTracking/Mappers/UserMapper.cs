using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
namespace Ems.AttendanceTracking.Mappers
{
    public static class UserMapper
    {
        public static List<UserCredentialModel> ToUserCredentialDetailsModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<UserCredentialModel>();

            while (readers.Read())
            {
                var model = new UserCredentialModel
                {
                    Id = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"]),

                    FullName = Convert.IsDBNull(readers["FullName"]) ? string.Empty : Convert.ToString(readers["FullName"]),
                    Email = Convert.IsDBNull(readers["Email"]) ? string.Empty : Convert.ToString(readers["Email"]),
                    ContactNo = Convert.IsDBNull(readers["ContactNo"]) ? string.Empty : Convert.ToString(readers["ContactNo"]),
                    CreatedAt = Convert.IsDBNull(readers["CreatedAt"]) ? string.Empty : Convert.ToDateTime(readers["CreatedAt"]).ToShortDateString(),
                    UserTypeId = Convert.ToInt32(readers["UserTypeId"]),
                    IsActive = Convert.ToBoolean(readers["IsActive"]),

                    //OrganizationId = Convert.IsDBNull(readers["OrganizationId"]) ? string.Empty : Convert.ToString(readers["OrganizationId"]),
                   // OrganizationName = Convert.IsDBNull(readers["OrganizationName"]) ? string.Empty : Convert.ToString(readers["OrganizationName"]),
                    LoginID = Convert.IsDBNull(readers["LoginID"]) ? string.Empty : Convert.ToString(readers["LoginID"])
                };

                models.Add(model);
            }

            return models;
        }
        public static List<UserCredentialModel> ToUserFullDetails(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<UserCredentialModel>();

            while (readers.Read())
            {
                var model = new UserCredentialModel
                {
                    Id = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"]),
                    FullName = Convert.IsDBNull(readers["FullName"]) ? string.Empty : Convert.ToString(readers["FullName"]),
                    OldPassword = Convert.IsDBNull(readers["Password"]) ? string.Empty : Convert.ToString(readers["Password"]),
                    Email = Convert.IsDBNull(readers["Email"]) ? string.Empty : Convert.ToString(readers["Email"]),
                    ContactNo = Convert.IsDBNull(readers["ContactNo"]) ? string.Empty : Convert.ToString(readers["ContactNo"]),
                    CreatedAt = Convert.IsDBNull(readers["CreatedAt"]) ? string.Empty : Convert.ToDateTime(readers["CreatedAt"]).ToShortDateString(),
                    UserTypeId = Convert.ToInt32(readers["UserTypeId"]),
                    IsActive = Convert.ToBoolean(readers["IsActive"]),
                    LoginID = Convert.IsDBNull(readers["LoginID"]) ? string.Empty : Convert.ToString(readers["LoginID"]),
                    Password = Convert.IsDBNull(readers["Password"]) ? string.Empty : Convert.ToString(readers["Password"]),
                    CompanyId = Convert.IsDBNull(readers["CompanyId"]) ? 0 : Convert.ToInt32(readers["CompanyId"]),
                    CompanyName = Convert.IsDBNull(readers["CompanyName"]) ? string.Empty : Convert.ToString(readers["CompanyName"]),
                    HasAccessQrCodeScan = Convert.IsDBNull(readers["HasAccessQrCodeScan"]) ? (bool?)false : Convert.ToBoolean(readers["HasAccessQrCodeScan"]),
                    Address = Convert.IsDBNull(readers["Address"]) ? string.Empty : Convert.ToString(readers["Address"]),
                    UniqueDeviceIdentifier = Convert.IsDBNull(readers["UniqueDeviceIdentifier"]) ? string.Empty : Convert.ToString(readers["UniqueDeviceIdentifier"]),
                    IsMultipleDevieAllow = Convert.IsDBNull(readers["IsMultipleDevieAllow"])?(bool?)false:Convert.ToBoolean(readers["IsMultipleDevieAllow"]),
                };

                models.Add(model);
            }

            return models;
        }
        public static List<UserCredentialModel> ToUserFullInfo(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<UserCredentialModel>();

            while (readers.Read())
            {
                var model = new UserCredentialModel
                {
                    Id = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"]),
                    FullName = Convert.IsDBNull(readers["FullName"]) ? string.Empty : Convert.ToString(readers["FullName"]),
                    OldPassword = Convert.IsDBNull(readers["Password"]) ? string.Empty : Convert.ToString(readers["Password"]),
                    Email = Convert.IsDBNull(readers["Email"]) ? string.Empty : Convert.ToString(readers["Email"]),
                    ContactNo = Convert.IsDBNull(readers["ContactNo"]) ? string.Empty : Convert.ToString(readers["ContactNo"]),
                    CreatedAt = Convert.IsDBNull(readers["CreatedAt"]) ? string.Empty : Convert.ToDateTime(readers["CreatedAt"]).ToShortDateString(),
                    UserTypeId = Convert.ToInt32(readers["UserTypeId"]),
                    IsActive = Convert.ToBoolean(readers["IsActive"]),
                    LoginID = Convert.IsDBNull(readers["LoginID"]) ? string.Empty : Convert.ToString(readers["LoginID"]),
                    Password = Convert.IsDBNull(readers["Password"]) ? string.Empty : Convert.ToString(readers["Password"]),
                    CompanyId = Convert.IsDBNull(readers["CompanyId"]) ? 0 : Convert.ToInt32(readers["CompanyId"]),
                    HasAccessQrCodeScan = Convert.IsDBNull(readers["HasAccessQrCodeScan"]) ? (bool?)false : Convert.ToBoolean(readers["HasAccessQrCodeScan"]),
                };

                models.Add(model);
            }

            return models;
        }

        public static List<UserCredentialModel> ToUserList(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<UserCredentialModel>();

            while (readers.Read())
            {
                var model = new UserCredentialModel
                {
                    Id = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"]),
                    FullName = Convert.IsDBNull(readers["FullName"]) ? string.Empty : Convert.ToString(readers["FullName"]),
                    Email = Convert.IsDBNull(readers["Email"]) ? string.Empty : Convert.ToString(readers["Email"]),
                    ContactNo = Convert.IsDBNull(readers["ContactNo"]) ? string.Empty : Convert.ToString(readers["ContactNo"]),
                    CreatedAt = Convert.IsDBNull(readers["CreatedAt"]) ? string.Empty : Convert.ToDateTime(readers["CreatedAt"]).ToShortDateString(),
                    UserTypeId = Convert.ToInt32(readers["UserTypeId"]),
                    IsActive = Convert.ToBoolean(readers["IsActive"]),
                    LoginID = Convert.IsDBNull(readers["LoginID"]) ? string.Empty : Convert.ToString(readers["LoginID"]),
                    CompanyId = Convert.IsDBNull(readers["CompanyId"]) ? 0 : Convert.ToInt32(readers["CompanyId"]),
                    CompanyName = Convert.IsDBNull(readers["CompanyName"]) ? string.Empty : Convert.ToString(readers["CompanyName"]),

                    LastloginTime = Convert.IsDBNull(readers["LastloginTime"]) ? string.Empty : Convert.ToDateTime(readers["LastloginTime"]).ToString(Constants.DateTimeLongFormat),
                    UniqueDeviceIdentifier = Convert.IsDBNull(readers["UniqueDeviceIdentifier"]) ? string.Empty : Convert.ToString(readers["UniqueDeviceIdentifier"]),
                    DeviceName = Convert.IsDBNull(readers["deviceName"]) ? string.Empty : Convert.ToString(readers["deviceName"]),
                    DeviceBrand = Convert.IsDBNull(readers["brand"]) ? string.Empty : Convert.ToString(readers["brand"]),
                    DeviceModelName = Convert.IsDBNull(readers["modelName"]) ? string.Empty : Convert.ToString(readers["modelName"])
                };

                models.Add(model);
            }

            return models;
        }

    }
}
