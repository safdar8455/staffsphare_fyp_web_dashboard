using System;
using System.Linq;
using System.Web.Mvc;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;
using Ems.AttendanceTracking.Models;
using Ems.AttendanceTracking.Services;
using Ems.BusinessTracker.Common.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class DepartmentController : BaseController
    {
        public DepartmentController()
        {
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetAll(GridSettings grid)
        {

            var query = new DepartmentBusiness().GetAll().AsQueryable();

            if (_userInfo.CompanyId.HasValue && _userInfo.CompanyId > 0)
                query = query.Where(x => x.CompanyId == _userInfo.CompanyId);

            var listOfFilteredData = FilterHelper.JQGridFilter(query, grid).AsQueryable();
            var listOfPagedData = FilterHelper.JQGridPageData(listOfFilteredData, grid);
            var count = listOfFilteredData.Count();
            var result = new
            {
                total = (int)Math.Ceiling((double)count / grid.PageSize),
                page = grid.PageIndex,
                records = count,
                rows = listOfPagedData
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(DepartmentModel model)
        {
            _ = new ResponseModel();
            model.CompanyId = _userInfo.CompanyId;
            ResponseModel response = new DepartmentBusiness().Save(model);
            return Json(response);
        }
        [HttpGet]
        public JsonResult Get(int id)
        {
            var model = new DepartmentBusiness().GetAll().FirstOrDefault(x => x.Id == id);
            if (model == null)
                model = new DepartmentModel();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            var response = new DepartmentBusiness().Delete(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}
