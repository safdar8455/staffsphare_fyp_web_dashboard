using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;


namespace Ems.AttendanceTracking.Services
{
	public class DepartmentBusiness
    {
		private readonly IDepartment _department;

		public DepartmentBusiness()
		{
			_department = AttendanceUnityMapper.GetInstance<IDepartment>();
		}
		public ResponseModel Save(DepartmentModel model)
		{
			return _department.Save(model);
		}
		public IEnumerable<DepartmentModel> GetAll()
		{
			var list = _department.GetAll();
			return list;
		}

		public ResponseModel Delete(int id)
		{
			return _department.Delete(id);
		}
	}
}
