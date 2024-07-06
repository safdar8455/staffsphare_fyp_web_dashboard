using Ems.BusinessTracker.Common;
using System;
using System.ComponentModel;

namespace Ems.AttendanceTracking.Models
{
    public class EmployeeCreateModel
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
        public string MobileNo { get; set; }
        public int? StatusId { get; set; }
        public int? LineManagerId { get; set; }
        public string LineManagerName { get; set; }
    }
    public class EmployeeDocExpiryReportModel
    {
        [DisplayName("EMP No")]
        public string EmployeeCode { get; set; }
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        [DisplayName("Mobile No")]
        public string MobileNo { get; set; }
        [DisplayName("Passport No")]
        public string PassportNo { get; set; }
        [DisplayName("Passport Expiry")]
        public string PassportExpiryDateVw { get; set; }
        [DisplayName("QID")]
        public string QID { get; set; }
        [DisplayName("QID Expiry")]
        public string QIDExpiryDateVw { get; set; }
        [DisplayName("Visa")]
        public string VisaNo { get; set; }
        [DisplayName("Visa Expiry")]
        public string VisaExpirayDate { get; set; }
        [DisplayName("Health Card")]
        public string HealthCardNo { get; set; }
        [DisplayName("Health Card Expiry")]
        public string HealthCardExpiryVw { get; set; }
    }
    public class EmployeeDocExpiryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Qid { get; set; }
        public int Visa { get; set; }
        public int Passport { get; set; }
        public int HealthCard { get; set; }
    }
    public class EmployeeDocExpiryDaysModel
    {
        public int QidExpiry { get; set; }
        public int QidExpiry90Days { get; set; }
        public int QidExpiry60Days { get; set; }
        public int QidExpiry30Days { get; set; }
        public int VisaExpiry { get; set; }
        public int VisaExpiry90Days { get; set; }
        public int VisaExpiry60Days { get; set; }
        public int VisaExpiry30Days { get; set; }
        public int PassportExpiry { get; set; }
        public int PassportExpiry90Days { get; set; }
        public int PassportExpiry60Days { get; set; }
        public int PassportExpiry30Days { get; set; }
        public int HealthExpiry { get; set; }
        public int HealthExpiry90Days { get; set; }
        public int HealthExpiry60Days { get; set; }
        public int HealthExpiry30Days { get; set; }
    }
    public class EmployeeEmailModel
    {
        public string EmployeeCode { get; set; }
        public string ExpiryNo { get; set; }
        public string ExpiryName { get; set; }
        public string ExpiryDays { get; set; }
        public string Email { get; set; }
    }
    public class EmployeeListModel
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string MobileNo { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get
            {
                return StatusId.HasValue ? EnumUtility.GetDescriptionFromEnumValue((EmployeeStatus)StatusId) : string.Empty;
            } }
        public string LineManagerName { get; set; }
    }

    public class EmployeeBatchUploadModel
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }


        public string Project { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PassportNo { get; set; }
        public DateTime? PassportIssueDate { get; set; }

        public DateTime? PassportExpiryDate { get; set; }
        public string QID { get; set; }
        public DateTime? QIDExpiryDate { get; set; }
        public string WorkingCompany { get; set; }
        public string Sponsorship { get; set; }
        public string VisaNo { get; set; }
        public DateTime? VisaExpirayDate { get; set; }

        public string WorkLocation { get; set; }
        public string CompanyAccomodation { get; set; }
        public string HealthCardNo { get; set; }
        public DateTime? HealthCardExpiryDate { get; set; }
        public string MobileNo { get; set; }
        public string Insurance { get; set; }
        public DateTime? InsuranceExpirayDate { get; set; }

        public string FoodHandling { get; set; }
        public DateTime? FoodhandlingIssueDate { get; set; }
        public DateTime? FoodhandlingExpiryDate { get; set; }


        public decimal? BasicPay { get; set; }
        public decimal? Housing { get; set; }
        public decimal? Transport { get; set; }
        public decimal? Telephone { get; set; }

        public decimal? FoodAllowance { get; set; }
        public decimal? OtherAllowancce { get; set; }
        public decimal? TeamLeadAllowance { get; set; }
        public decimal? CityCompensatoryAllowance { get; set; }
        public decimal? PersonalAllowance { get; set; }
        public decimal? OutsideAllowance { get; set; }
        public decimal? NetSalary { get; set; }

        public string LeavePolicyCode { get; set; }
        public int? LeaveEntitlement { get; set; }
        public int? AirTicketEntitlementTotalMonth { get; set; }
        public string HiredThrough { get; set; }
        public int? ContractPeriodYear { get; set; }
        public string LaborContract { get; set; }
        public DateTime? ContractIssueDate { get; set; }

        public DateTime? ContractExpiryDate { get; set; }
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
        public string UpdaLicense { get; set; }
        public string Registration { get; set; }
        public string Grade { get; set; }
        public DateTime? UdpaExpiryDate { get; set; }
        public DateTime? LastWorkingDate { get; set; }

        public string Status { get; set; }
        public string Remarks { get; set; }

        public string ActionById { get; set; }
        public int? WorkingCompanyId { get; set; }

    }
}
