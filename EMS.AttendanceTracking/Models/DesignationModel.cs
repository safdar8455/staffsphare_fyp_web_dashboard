﻿namespace Ems.AttendanceTracking.Models
{
    public class DesignationModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
    }
}
