using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface IHoliday
    {
        ResponseModel Save(HolidayModel model);
        List<HolidayModel> GetAll(int? cId);
        HolidayModel GetById(int id);
        ResponseModel Delete(int id);
    }
}
