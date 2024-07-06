using Ems.BusinessTracker.Common;
using System;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace Ems.AttendanceTracking.Models
{
    public class EmployeeExportModel
    {
        [Browsable(false)]
        public int PdfSerialNo { get; set; }
        [Browsable(false)]
        public byte[] EmployeeImageData { get; set; }
        [Browsable(false)]
        public bool? IsDeleteable { get; set; }
        public byte[] QrCode { get; set; }
        [Browsable(false)]
        public string QrCodeNo { get; set; }
        [Browsable(false)]
        public long Id { get; set; }
        public string PortalUserId { get; set; }
        [Browsable(false)]
        public bool? HasAccessQrCodeScan { get; set; }
        [DisplayName("Sr.")]
        public int SerialNo { get; set; }
        [DisplayName("EMP NO")]
        public string EmployeeCode { get; set; }
        [DisplayName("NAME")]
        public string EmployeeName { get; set; }
        [DisplayName("Login ID")]
        public string LoginID { get; set; }
        [DisplayName("DESIGNATION")]
        public string Designation { get; set; }
        [DisplayName("DEPARTMENT")]
        public string Department { get; set; }
        [Browsable(false)]
        [ScriptIgnore]
        public DateTime? JoiningDate { get; set; }
        [DisplayName("JOINING DATE")]
        public string JoiningDateVw 
        {
            get { return JoiningDate.HasValue ? JoiningDate.Value.ToString(Constants.DateFormat):string.Empty; }
        }

        [DisplayName("NATIONALITY")]
        public string Nationality { get; set; }
        [Browsable(false)]
        [ScriptIgnore]
        public DateTime? DateOfBirth { get; set; }
        [DisplayName("DATE OF BIRTH")]
        public string DateOfBirthVw
        {
            get { return DateOfBirth.HasValue ? DateOfBirth.Value.ToString(Constants.DateFormat) : string.Empty; }
        }
        [Browsable(false)]
        public int EmployeeAge
        {
            get
            {
                if (DateOfBirth == null)
                    return 0;
                return (DateTime.UtcNow.Year - DateOfBirth.Value.Year);
            }
        }

        [DisplayName("GENDER")]
        public string Gender { get; set; }
        [DisplayName("WORKING COMPANY")]
        public string WorkingCompany { get; set; }
        [DisplayName("WORK LOCATION")]
        public string WorkLocation { get; set; }
        [DisplayName("HEALTH CARD NO")]
        public string HealthCardNo { get; set; }
        
        [Browsable(false)]
        [ScriptIgnore]
        public DateTime? HealthCardExpiry { get; set; }

        [DisplayName("HEALTH CARD EXPIRY")]
        public string HealthCardExpiryVw
        {
            get { return HealthCardExpiry.HasValue ? HealthCardExpiry.Value.ToString(Constants.DateFormat) : string.Empty; }
        }
        [DisplayName("CONTACT NO")]
        public string MobileNo { get; set; }
        [DisplayName("INSURANCE")]
        public string Insurance { get; set; }
        
        [Browsable(false)]
        [ScriptIgnore]
        public DateTime? InsuranceExpiryDate { get; set; }
        [DisplayName("INSURANCE EXPIRY DATE")]
        public string InsuranceExpiryDateVw
        {
            get { return InsuranceExpiryDate.HasValue ? InsuranceExpiryDate.Value.ToString(Constants.DateFormat) : string.Empty; }
        }
        [DisplayName("BASIC PAY")]
        public decimal? BasicPay { get; set; }
        [DisplayName("HOUSING")]
        public decimal? Housing { get; set; }
        [DisplayName("TRANSPORT")]
        public decimal? Transport { get; set; }
        [DisplayName("TELEPHONE")]
        public decimal? Telephone { get; set; }
        [DisplayName("FOOD ALLOWANCE")]
        public decimal? FoodAllowance { get; set; }
        [DisplayName("OTHER ALLOWANCES")]
        public decimal? OtherAllowances { get; set; }
        [DisplayName("NET SALARY")]
        public decimal? NetSalary { get; set; }
        [Browsable(false)]
        [ScriptIgnore]
        public DateTime? ContractIssueDate { get; set; }
        [DisplayName("CONTRACT ISSUE DATE")]
        public string ContractIssueDateVw
        {
            get { return ContractIssueDate.HasValue ? ContractIssueDate.Value.ToString(Constants.DateFormat) : string.Empty; }
        }


        
        [Browsable(false)]
        [ScriptIgnore]
        public DateTime? ContractExpiryDate { get; set; }
        [DisplayName("CONTRACT EXPIRY DATE")]
        public string ContractExpiryDateVw
        {
            get { return ContractExpiryDate.HasValue ? ContractExpiryDate.Value.ToString(Constants.DateFormat) : string.Empty; }
        }



        [DisplayName("COMPANY ID")]
        public string CompanyID { get; set; }
        [DisplayName("SALARY CATEGORY")]
        public string SalaryCategory { get; set; }
        [DisplayName("BANK NAME")]
        public string BankName { get; set; }
        [DisplayName("EMPLOYEE ACCOUNT")]
        public string EmployeeAccount { get; set; }
        [DisplayName("MOTHER TONGUE")]
        public string MotherTongue { get; set; }
        [DisplayName("MARITAL STATUS")]
        public string MaritalStatus { get; set; }
        [DisplayName("RELIOGION")]
        public string Religion { get; set; }
        [DisplayName("PREVIOUS COMPANY")]
        public string PreviousCompany { get; set; }
        [DisplayName("COUNTRY")]
        public string Country { get; set; }
        [DisplayName("EMAIL ID")]
        public string EmailIDs { get; set; }
        [DisplayName("GRADE")]
        public string Grade { get; set; }
        [Browsable(false)]
        [ScriptIgnore]
        public DateTime? LastWorkngDate { get; set; }
        [DisplayName("LAST WORKING DATE")]
        public string LastWorkngDateVw
        {
            get { return LastWorkngDate.HasValue ? LastWorkngDate.Value.ToString(Constants.DateFormat) : string.Empty; }
        }

        [Browsable(false)]
        public int? StatusId { get; set; }
        [Browsable(false)]
        public string CompanyAdminEmail { get; set; }
        [DisplayName("STATUS")]
        public string StatusName
        {
            get
            {
                return StatusId.HasValue ? EnumUtility.GetDescriptionFromEnumValue((EmployeeStatus)StatusId) : string.Empty;
            }
        }
        [DisplayName("REMARKS")]
        public string Remarks { get; set; }




        [Browsable(false)]
        public int? LeavePolicyId { get; set; }
        [Browsable(false)]
        public int? MaritalStatusId { get; set; }
        [Browsable(false)]
        public int? ReligionId { get; set; }
        [Browsable(false)]
        public int? CountryId { get; set; }
        [Browsable(false)]
        public int? DesignationId { get; set; }
        [Browsable(false)]
        public int? DepartmentId { get; set; }
        [Browsable(false)]
        public int? ProjectId { get; set; }
        [Browsable(false)]
        public int? NationalityId { get; set; }
        [Browsable(false)]
        public int? GenderId { get; set; }
        [Browsable(false)]
        public int? WorkingCompanyId { get; set; }
        [Browsable(false)]
        public int? WorkLocationId { get; set; }
        [Browsable(false)]
        public string ImageFileName { get; set; }
        [Browsable(false)]
        public string ImagePath { get; set; }

    }
}
