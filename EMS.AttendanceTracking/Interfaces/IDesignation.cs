using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface IDesignation
    {
        ResponseModel Save(DesignationModel model);
        List<DesignationModel> GetAll(int companyId);
        ResponseModel Delete(int id);
    }
}
