using Ems.BusinessTracker.Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Mappers
{
    public class CompanyMapper
    {
        public static List<AttachmentModel> ToAttachFilesModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<AttachmentModel>();

            while (readers.Read())
            {
                var model = new AttachmentModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    CompanyId = Convert.ToInt32(readers["CompanyId"]),
                    BlobName = Convert.IsDBNull(readers["BlobName"]) ? string.Empty : Convert.ToString(readers["BlobName"]),
                    FileName = Convert.IsDBNull(readers["FileName"]) ? string.Empty : Convert.ToString(readers["FileName"]),
                    AttachmentTypeId = Convert.IsDBNull(readers["AttachmentTypeId"]) ? (int?)null : Convert.ToInt32(readers["AttachmentTypeId"])
                };
                model.UploadedFileFullPath = Constants.LocalFilePath + model.BlobName;
                models.Add(model);
            }
            return models;
        }
        public static List<Company> ToCompanyModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<Company>();

            while (readers.Read())
            {
                var model = new Company
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    CompanyName = Convert.IsDBNull(readers["CompanyName"]) ? string.Empty : Convert.ToString(readers["CompanyName"]),
                    Address = Convert.IsDBNull(readers["Address"]) ? string.Empty : Convert.ToString(readers["Address"]),
                    HrDirectorCode = Convert.IsDBNull(readers["HrDirectorCode"]) ? string.Empty : Convert.ToString(readers["HrDirectorCode"]),
                    HrDirectorName = Convert.IsDBNull(readers["EmployeeName"]) ? string.Empty : Convert.ToString(readers["EmployeeName"]),
                    PhoneNumber = Convert.IsDBNull(readers["PhoneNumber"]) ? string.Empty : Convert.ToString(readers["PhoneNumber"]),
                    ImageFileName = Convert.IsDBNull(readers["ImageFileName"]) ? string.Empty : Convert.ToString(readers["ImageFileName"]),
                    ImageFileId = Convert.IsDBNull(readers["ImageFileId"]) ? string.Empty : Convert.ToString(readers["ImageFileId"]),
                    CreatedById = Convert.IsDBNull(readers["CreatedById"]) ? string.Empty : Convert.ToString(readers["CreatedById"]),
                    CreatedDate = Convert.IsDBNull(readers["CreatedDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["CreatedDate"]),
                    IsActive = Convert.IsDBNull(readers["IsActive"]) ? false : Convert.ToBoolean(readers["IsActive"]),
                    IsMultipleDevieAllow = Convert.IsDBNull(readers["IsMultipleDevieAllow"]) ? false : Convert.ToBoolean(readers["IsMultipleDevieAllow"]),
                    CompanyAdminName = Convert.IsDBNull(readers["CompanyAdminName"]) ? string.Empty : Convert.ToString(readers["CompanyAdminName"]),
                    CompanyAdminEmail = Convert.IsDBNull(readers["CompanyAdminEmail"]) ? string.Empty : Convert.ToString(readers["CompanyAdminEmail"]),
                    CompanyAdminLoginID = Convert.IsDBNull(readers["CompanyAdminLoginID"]) ? string.Empty : Convert.ToString(readers["CompanyAdminLoginID"]),
                    CompanyRegistrationNumber = Convert.IsDBNull(readers["CompanyRegistrationNumber"]) ? string.Empty : Convert.ToString(readers["CompanyRegistrationNumber"]),
                    TradeLicenseNumber = Convert.IsDBNull(readers["TradeLicenseNumber"]) ? string.Empty : Convert.ToString(readers["TradeLicenseNumber"]),
                    EstablishmentCardNumber = Convert.IsDBNull(readers["EstablishmentCardNumber"]) ? string.Empty : Convert.ToString(readers["EstablishmentCardNumber"]),
                    CompanyRegistrationExpiresInDays = Convert.IsDBNull(readers["CompanyRegistrationExpiresInDays"]) ? (int?)null : Convert.ToInt32(readers["CompanyRegistrationExpiresInDays"]),
                    EstablishmentCardExpiresInDays = Convert.IsDBNull(readers["EstablishmentCardExpiresInDays"]) ? (int?)null : Convert.ToInt32(readers["EstablishmentCardExpiresInDays"]),
                    TradeLicenseExpiresInDays = Convert.IsDBNull(readers["TradeLicenseExpiresInDays"]) ? (int?)null : Convert.ToInt32(readers["TradeLicenseExpiresInDays"]),
                    OthersExpiresInDays = Convert.IsDBNull(readers["OthersExpiresInDays"]) ? (int?)null : Convert.ToInt32(readers["OthersExpiresInDays"]),
                    CompanyRegistrationExpiryDateVw = Convert.IsDBNull(readers["CompanyRegistrationExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["CompanyRegistrationExpiryDate"]).ToString(Constants.DateFormat),
                    EstablishmentCardExpiryDateVw = Convert.IsDBNull(readers["EstablishmentCardExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["EstablishmentCardExpiryDate"]).ToString(Constants.DateFormat),
                    TradeLicenseExpiryDateVw = Convert.IsDBNull(readers["TradeLicenseExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["TradeLicenseExpiryDate"]).ToString(Constants.DateFormat),
                    OthersExpiryDateVw = Convert.IsDBNull(readers["OthersExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["OthersExpiryDate"]).ToString(Constants.DateFormat),
                };

                models.Add(model);
            }

            return models;
        }
    }
}
