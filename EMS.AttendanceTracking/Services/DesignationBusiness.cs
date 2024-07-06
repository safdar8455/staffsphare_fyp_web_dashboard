using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;


namespace Ems.AttendanceTracking.Services
{
	public class DesignationBusiness
	{
		private readonly IDesignation _Designation;

		public DesignationBusiness()
		{
			_Designation = AttendanceUnityMapper.GetInstance<IDesignation>();
		}
		public ResponseModel Save(DesignationModel model)
		{
			return _Designation.Save(model);
		}
		public IEnumerable<DesignationModel> GetAll(int cId)
		{
			var list = _Designation.GetAll(cId);
			return list;
		}

		public ResponseModel Delete(int id)
		{
			return _Designation.Delete(id);
		}
	}
}
