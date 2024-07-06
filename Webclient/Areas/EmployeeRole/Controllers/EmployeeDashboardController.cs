using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;
using Ems.AttendanceTracking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;

namespace Webclient.Areas.EmployeeRole.Controllers
{
    [SessionHelper]
    public class EmployeeDashboardController : BaseController
    {
        private readonly IAttendance _attendance;
        private readonly IEmployee _Employee;
        public EmployeeDashboardController()
        {
            _attendance = AttendanceUnityMapper.GetInstance<IAttendance>();
            _Employee = AttendanceUnityMapper.GetInstance<IEmployee>();
        }
        public ActionResult Index()
        {
            return View();
        }
       

    }
}
