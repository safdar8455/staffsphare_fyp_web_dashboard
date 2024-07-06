using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;

namespace Ems.AttendanceTracking.Models
{
    public class DepartmentModel
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string Code { get; set; }
        public string DepartmentName { get; set; }
        public string LineManagerCode { get; set; }
        public string DepartmentManagerCode { get; set; }
        public string LineManagerName { get; set; }
        public string DepartmentManagerName { get; set; }
        public string CompanyName { get; set; }
        public List<NameIdPairModel> CompanyList { get; set; }
    }
}
