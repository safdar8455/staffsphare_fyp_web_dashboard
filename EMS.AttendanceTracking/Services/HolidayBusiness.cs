using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;


namespace Ems.AttendanceTracking.Services
{
	public class HolidayBusiness
	{
		private readonly IHoliday _holiday;

		public HolidayBusiness()
		{
			_holiday = AttendanceUnityMapper.GetInstance<IHoliday>();
		}
		public ResponseModel Save(HolidayModel model)
		{
			return _holiday.Save(model);
		}
		public IEnumerable<HolidayModel> GetAll(int? cId)
		{
			var list = _holiday.GetAll(cId);
			return list;
		}

		public HolidayModel GetById(int id)
		{
			return _holiday.GetById(id);
		}

		public ResponseModel Delete(int id)
		{
			return _holiday.Delete(id);
		}
	}
}
