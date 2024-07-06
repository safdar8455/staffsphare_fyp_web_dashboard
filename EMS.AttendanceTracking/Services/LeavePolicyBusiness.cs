using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;


namespace Ems.AttendanceTracking.Services
{
	public class LeavePolicyBusiness
	{
		private readonly ILeavePolicy _LeavePolicy;

		public LeavePolicyBusiness()
		{
			_LeavePolicy = AttendanceUnityMapper.GetInstance<ILeavePolicy>();
		}
		public ResponseModel Save(LeavePolicyModel model)
		{
			return _LeavePolicy.Save(model);
		}
		public IEnumerable<LeavePolicyModel> GetAll()
		{
			var list = _LeavePolicy.GetAll();
			return list;
		}

		public ResponseModel Delete(int id)
		{
			return _LeavePolicy.Delete(id);
		}
	}
}
