using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;

namespace Ems.AttendanceTracking.Models
{
    public class EmployeeDetailsModel
    {
        public int Id { get; set; }
        public int SerialNo { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string LineManager { get; set; }
        public string DepartmentManager { get; set; }
        public string EmployeeHrDirector { get; set; }
        public string Project { get; set; }
        
        
        public DateTime? JoiningDate { get; set; }
        public string JoiningDateVw { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthVw { get; set; }
        public DateTime? PassportIssueDate { get; set; }
        public string PassportIssueDateVw { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public string PassportExpiryDateVw { get; set; }
        public DateTime? QIDExpiryDate { get; set; }
        public string QIDExpiryDateVw { get; set; }
        public DateTime? VisaExpirayDate { get; set; }
        public string VisaExpiryDateVw { get; set; }
        public DateTime? HealthCardExpiry { get; set; }
        public string HealthCardExpiryVw { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public string InsuranceExpiryDateVw { get; set; }
        public DateTime? FoodHandlingIssueDate { get; set; }
        public string FoodHandlingIssueDateVw { get; set; }
        public DateTime? FoodhandlingExpiryDate { get; set; }
        public string FoodhandlingExpiryDateVw { get; set; }
        public DateTime? ContractIssueDate { get; set; }
        public string ContractIssueDateVw { get; set; }

        public DateTime? ContractExpiryDate { get; set; }
        public string ContractExpiryDateVw { get; set; }
        public DateTime? UpdaExpiryDate { get; set; }
        public string UpdaExpiryDateVw { get; set; }
        public DateTime? LastWorkngDate { get; set; }
        public string LastWorkngDateVw { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string PassportNo { get; set; }
        
        public decimal? PassportInfoUpdateOnQidCost { get; set; }
        public string QID { get; set; }
        public decimal? RpFines { get; set; }
        public decimal? ResidencePermitIssuanceCost { get; set; }
        public string WorkingCompany { get; set; }
        public string Sponsorship { get; set; }
        public string VisaNo { get; set; }
        public string WorkLocation { get; set; }
        public string CompanyAccomodation { get; set; }
        public string HealthCardNo { get; set; }
        public decimal? HealthCardIssuanceCost { get; set; }
        public string ContactNo { get; set; }
        public string Insurance { get; set; }
        public string FoodHandling { get; set; }
        public decimal? BasicPay { get; set; }
        public decimal? Housing { get; set; }
        public decimal? Transport { get; set; }
        public decimal? Telephone { get; set; }
        public decimal? FoodAllowance { get; set; }
        public decimal? OtherAllowances { get; set; }
        public decimal? TeamLeaderAllowance { get; set; }
        public decimal? CityCompensatoryAllowance { get; set; }
        public decimal? PersonalAllowance { get; set; }
        public decimal? OutSideAllowance { get; set; }
        public decimal? NetSalary { get; set; }
        public string LeavePolicy { get; set; }
        public string LeavePolicyCode { get; set; }
        public string LeaveEntitlement { get; set; }
        public string AirTicketEntitlement { get; set; }
        public string HiredThrough { get; set; }
        public string ContractPeriod { get; set; }
        public string LaborContract { get; set; }
        public string NewContractCost { get; set; }
        public string CompanyID { get; set; }
        public string SalaryCategory { get; set; }
        public string BankName { get; set; }
        public string EmployeeAccount { get; set; }
        public string MotherTongue { get; set; }
        public string MaritalStatus { get; set; }
        public int? ChildrenNo { get; set; }
        public string Religion { get; set; }
        public string PreviousCompany { get; set; }
        public string Country { get; set; }
        public string HomeAirport { get; set; }
        public string CompanyEmailID { get; set; }
        public string EmailIDs { get; set; }
        public string EmployeeGroup { get; set; }
        public string EmployeeSubGroup { get; set; }
        public string EmployeeOfTheMonth { get; set; }
        public string EpdaLicense { get; set; }
        public string Registration { get; set; }
        public string Grade { get; set; }
        public int? EmployeeStatusId { get; set; }
        public string StatusName
        {
            get
            {
                return EmployeeStatusId.HasValue ? EnumUtility.GetDescriptionFromEnumValue((EmployeeStatus)EmployeeStatusId) : string.Empty;
            }
        }
        public string Remarks { get; set; }
        public int? LeavePolicyId { get; set; }
        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
        public int? NationalityId { get; set; }
        public int? GenderId { get; set; }
        public int? WorkingCompanyId { get; set; }
        public int? SponsorshipId { get; set; }
        public int? WorkLocationId { get; set; }
        public int? ProjectId { get; set; }
        public int? MotherTongueId { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? ReligionId { get; set; }
        public int? CountryId { get; set; }
        public int? HomeAirportId { get; set; }
        public int? EmployeeGroupId { get; set; }
        public string ActionById { get; set; }
        public bool? HasAccessQrCodeScan { get; set; }
        public bool? IsDeleteable { get; set; }
        public DateTime? HealthCardExpiryDate { get; set; }
        public string MobileNo { get; set; }
        public List<NameIdPairModel> DesignationList { get; set; }
        public List<NameIdPairModel> DepartmentList { get; set; }
        public List<NameIdPairModel> NationalityList { get; set; }
        public List<NameIdPairModel> GenderList { get; set; }
        public List<NameIdPairModel> SponsorCompanyList { get; set; }
        public List<NameIdPairModel> WorkingLocationList { get; set; }
        public List<NameIdPairModel> MotherTongueList { get; set; }
        public List<NameIdPairModel> ReligionList { get; set; }
        public List<NameIdPairModel> CountryList { get; set; }
        public List<NameIdPairModel> HomeAirportList { get; set; }
        public List<NameIdPairModel> EmployeeGroupList { get; set; }
        public List<NameIdPairModel> MaritalStatusList { get; set; }
        public List<NameIdPairModel> ProjectList { get; set; }
        public List<NameIdPairModel> EmployeeStatusList { get; set; }
        public string ImageFileName { get; set; }
        public string ImagePath { get; set; }
        public LocalDocumentModel AttachedDocument { get; set; }
    }
    public class LocalDocumentModel
    {
        public string UploadedFileName { get; set; }
        public string UploadedFileFullPath { get; set; }
        public string DisplayFileName { get; set; }
        public string AzureUrl { get; set; }
    }
}
