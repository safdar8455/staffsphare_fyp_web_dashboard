using System;
using System.ComponentModel.DataAnnotations;

namespace Ems.BusinessTracker.Common.Models
{

    public class UserCredentialModel
    {
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required(ErrorMessage = "The Login ID field is required")]
        public string LoginID { get; set; }
        [Required(ErrorMessage = "The Full Name field is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "The Contact No field is required")]
        public string ContactNo { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public int UserTypeId { get; set; }
        public bool IsActive { get; set; }

        public string CreatedAt { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }

        public string StatusName
        {
            get { return IsActive ? "Enabled" : "Disabled"; }
        }

        public string RoleName
        {
            get
            {
                return UserTypeId > 0 ? EnumUtility.GetDescriptionFromEnumValue((UserType)UserTypeId) : string.Empty;
            }
        }
        public bool? HasAccessQrCodeScan { get; set; }
        public string OldPassword { get; set; }
        public bool? IsFirstLogin { get; set; }
        public bool? IsMultipleDevieAllow { get; set; }
        public string UniqueDeviceIdentifier { get; set; }

        public string LastloginTime { get; set; }
        public string DeviceName { get; set; }
        public string DeviceBrand { get; set; }
        public string DeviceModelName { get; set; }

    }

    public class ResetPasswordModel
    {

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class UserSessionModel
    {
        public string Id { get; set; }
        public int UserTypeId { get; set; }
        public string FullName { get; set; }
        public string UserInitial { get; set; }
        public string Email { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyLogo { get; set; }

    }
    public class DeviceInformation
    {
        public string userId { get; set; }
        public string deviceName { get; set; }
        public string brand { get; set; }
        public string modelName { get; set; }
        public string osName { get; set; }
        public string osVersion { get; set; }
        public string osBuildId { get; set; }
        public string UniqueDeviceIdentifier { get; set; }
        public DateTime? LastloginTime { get; set; }
    }

}
