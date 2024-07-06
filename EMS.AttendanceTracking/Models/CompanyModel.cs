using System;

namespace Ems.AttendanceTracking.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }

        public string PhoneNumber { get; set; }
        public string ImageFileName { get; set; }
        public string ImageFileId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public bool? IsActive { get; set; }
        public string Status
        {
            get { return IsActive.HasValue && IsActive.Value ? "Active" : "InActive"; }
        }

        public string CompanyAdminName { get; set; }
        public string CompanyAdminEmail { get; set; }
        public string CompanyAdminLoginID { get; set; }
        public string CompanyAdminPassword { get; set; }
        public string HrDirectorCode { get; set; }
    }
}
