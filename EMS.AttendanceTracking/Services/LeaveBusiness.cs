using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;
using System.Linq;

namespace Ems.AttendanceTracking.Services
{
	public class LeaveBusiness
	{
		private readonly ILeave _leave;
		public LeaveBusiness()
		{
			_leave = AttendanceUnityMapper.GetInstance<ILeave>();
		}
		public List<LeaveModel> GetLeaveByCompanyId(int companyId)
		{
			return _leave.GetLeaveByCompanyId(companyId).ToList();
		}
		public List<LeaveModel> GetUserPendingLeaves(string userId)
		{
			var list = _leave.GetUserPendingLeaves(userId);
			return list;
		}
		public LeaveModel GetLeaveById(int Id,string userId)
		{
			var list = _leave.GetLeaveById(Id);
				list.CanApprove = list.NextApproverId == userId;
			return list;
		}
		public ResponseModel ApproveOrRejectLeave(LeaveApproveModel model)
		{
			var response = _leave.ApproveOrRejectLeave(model);
			return response;
		}
	}
}
