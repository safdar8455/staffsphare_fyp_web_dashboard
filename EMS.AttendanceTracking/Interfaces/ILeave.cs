using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface ILeave
    {
        List<LeaveModel> GetLeaveByCompanyId(int companyId);
        LeaveModel GetLeaveById(int Id); 
        List<LeaveModel> GetUserLeaves(string userId);
        List<LeaveModel> GetUserPendingLeaves(string userId);
        ResponseModel CreateEmployeeLeave(LeaveModel model);
        ResponseModel Approved(int id, string userId); 
        ResponseModel Rejected(int id);
        ResponseModel Correction(int id);
        ResponseModel ApproveOrRejectLeave(LeaveApproveModel model);
    }
}
