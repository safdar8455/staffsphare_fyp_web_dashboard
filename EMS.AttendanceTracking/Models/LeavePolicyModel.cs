namespace Ems.AttendanceTracking.Models
{
    public class LeavePolicyModel
    {
        public int Id { get; set; }
        public string PolicyCode { get; set; }
        public string Description { get; set; }
        public int? LeaveDays { get; set; }
    }
}
