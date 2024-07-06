using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface ILeavePolicy
    {
        ResponseModel Save(LeavePolicyModel model);
        List<LeavePolicyModel> GetAll();
        ResponseModel Delete(int id);
    }
}
