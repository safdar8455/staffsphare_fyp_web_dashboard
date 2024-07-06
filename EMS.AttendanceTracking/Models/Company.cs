using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;

namespace Ems.AttendanceTracking.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageFileName { get; set; }
        public string ImageFileId { get; set; }
        [ScriptIgnore]
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public bool? IsActive { get; set; }
        public string Status
        {
            get { return IsActive.HasValue && IsActive.Value ? "Active" : "InActive"; }
        }

        public bool? IsMultipleDevieAllow { get; set; }

        public string MultipleDevieAllowVw
        {
            get { return IsMultipleDevieAllow.HasValue && IsMultipleDevieAllow.Value ? "Yes" : "No"; }
        }

        public string CompanyAdminName { get; set; }
        public string CompanyAdminEmail { get; set; }
        public string CompanyAdminLoginID { get; set; }
        public string CompanyAdminPassword { get; set; }
        public string AdminAssignedPassword { get; set; }
        public string HrDirectorCode { get; set; }
        public string HrDirectorName { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        [ScriptIgnore]
        public DateTime? CompanyRegistrationExpiryDate { get; set; }
        public string CompanyRegistrationExpiryDateVw { get; set; }
        public int? CompanyRegistrationExpiresInDays { get; set; }
        public string EstablishmentCardNumber { get; set; }
        [ScriptIgnore]
        public DateTime? EstablishmentCardExpiryDate { get; set; }
        public string EstablishmentCardExpiryDateVw { get; set; }
        public int? EstablishmentCardExpiresInDays { get; set; }
        public string TradeLicenseNumber { get; set; }
        [ScriptIgnore]
        public DateTime? TradeLicenseExpiryDate { get; set; }
        public string TradeLicenseExpiryDateVw { get; set; }
        public int? TradeLicenseExpiresInDays { get; set; }
        [ScriptIgnore]
        public DateTime? OthersExpiryDate { get; set; }
        public string OthersExpiryDateVw { get; set; }
        public int? OthersExpiresInDays { get; set; }
        public List<CodeNamePairModel> EmployeeList { get; set; }
        public List<AttachmentModel> DocumentList { get; set; }
        public List<AttachmentModel> AttachedDocumentList { get; set; }
    }
    public class AttachmentModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string BlobName { get; set; }
        public string FileName { get; set; }
        public string UpdatedAt { get; set; }
        public int? UpdatedById { get; set; }
        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public int? AttachmentTypeId { get; set; }
        public string AttachmentType
        {
            get
            {
                if (AttachmentTypeId.HasValue && AttachmentTypeId > 0)
                    return EnumUtility.GetDescriptionFromEnumValue((CompanyAttachmentType)AttachmentTypeId);
                else
                    return string.Empty;
            }
        }
        public string UploadedFileFullPath { get; set; }

      
    }
}
