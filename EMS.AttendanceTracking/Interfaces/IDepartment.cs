using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface IDepartment
    {
        ResponseModel Save(DepartmentModel model);
        List<DepartmentModel> GetAll();
        ResponseModel Delete(int id);
    }
}
