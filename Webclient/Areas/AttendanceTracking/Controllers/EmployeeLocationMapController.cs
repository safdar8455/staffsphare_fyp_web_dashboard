using System;
using System.Linq;
using System.Web.Mvc;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Webclient.Controllers;
using Webclient.Filters;


namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class EmployeeLocationMapController : BaseController
    {

        private readonly IAttendance _attendance;
        public EmployeeLocationMapController()
        {
            _attendance = AttendanceUnityMapper.GetInstance<IAttendance>();
        }

        public ActionResult LiveTracking()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetEmployeeCurrentLatLong()
        {
            var result = _attendance.GetMovementDetailsAll(DateTime.UtcNow, this._userInfo.CompanyId.HasValue && this._userInfo.CompanyId > 0 ? this._userInfo.CompanyId : null).OrderBy(x => x.LogDateTime);
            var data = (from x in result
                        select new
                        {
                            x.Latitude,
                            x.Longitude,
                            x.UserName,
                            x.LogLocation,
                            x.Note
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
