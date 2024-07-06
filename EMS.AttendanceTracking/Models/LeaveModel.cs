using Ems.BusinessTracker.Common;
using System;
using System.ComponentModel;
namespace Ems.AttendanceTracking.Models
{
    public class LeaveModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool? IsHalfDay { get; set; }
        public int LeaveTypeId { get; set; }
        public int? ApproverSerialId { get; set; }
        public int? StatusId { get; set; }
        public string LeaveReason { get; set; }
        public string CreatedAt { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsCorrection { get; set; }

        public string RejectReason { get; set; }
        public string ApprovedById { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
        public string NextApproverId { get; set; }
        public bool CanApprove { get; set; }
        public string LeaveApplyFrom { get; set; }
        public string LeaveApplyTo { get; set; }
        public string UserId { get; set; }
        public string RequestNo { get; set; }

        public string LeaveType { get; set; }
        public string StatusName
        {
            get
            {
                if (StatusId == null)
                    return string.Empty;
                return EnumUtility.GetDescriptionFromEnumValue((LeaveStatus)StatusId);
            }
        }
        public int LeaveInDays
        {
            get { return ((int)ToDate.Subtract(FromDate).TotalDays) + 1; }
        }
        public string FromDateVw
        {
            get { return FromDate.ToString(Constants.DateLongFormat); }
        }
        public string ApprovedAtVw
        {
            get { return ApprovedAt.HasValue ? ApprovedAt.Value.ToString(Constants.DateLongFormat) : string.Empty; }
        }
        public string ToDateVw
        {
            get { return ToDate.ToString(Constants.DateLongFormat); }
        }
    }
    public class LeaveApproveModel
    {
        public int Id { get; set; }
        public string RequestNo { get; set; }
        public string ApproverId { get; set; }
        public bool? IsLastApprover { get; set; }
        public int? SerialNo { get; set; }
        public bool? Approved { get; set; }
        public bool? Rejected { get; set; }
        public string RejectReason { get; set; }
        public string UpdatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public enum LeaveStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3
    }
}
