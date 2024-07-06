using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Excel;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ems.AttendanceTracking.Models;
using Ems.AttendanceTracking.Services;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class EmployeeController : BaseReportController
    {
        private readonly EmployeeBusiness _eBusiness;
        public EmployeeController()
        {
            _eBusiness = new EmployeeBusiness(this._userInfo.Id);
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BulkUpload()
        {
            return View();
        }

        public ActionResult Create()
        {
            return PartialView();
        }

        public ActionResult Details()
        {
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetAll(GridSettings grid)
        {

            var query = _eBusiness.GetAll().AsQueryable();
            if (_userInfo.CompanyId.HasValue && _userInfo.CompanyId > 0)
                query = query.Where(x=>x.WorkingCompanyId==_userInfo.CompanyId);
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

        public void ExportEmployeeList( GridSettings grid)
        {
            var employeeList = _eBusiness.GetEmployeeExportList();

            var listOfFilteredRequest = FilterHelper.JQGridFilter(employeeList.AsQueryable(), grid).ToList();

            ExportToExcelAsFormated(listOfFilteredRequest.OrderBy(x=>x.SerialNo).ToList(), "Employee_List_" + DateTime.Now, null);
        }

        [HttpPost]
        public ActionResult Save(EmployeeDetailsModel model)
        {
            model.WorkingCompanyId = _userInfo.CompanyId;
            return Json(_eBusiness.Save(model));
        }
        [HttpGet]
        public JsonResult Get(long id)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            return Json(_eBusiness.GetEmployeeDetails(id,cId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            var response = _eBusiness.Delete(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadEmployeeBatchExcelFile()
        {
            var response = new ResponseModel();
            if (Request.Files.AllKeys.Any())
            {
                var httpPostedFile = Request.Files[0];
                var fileExtension = Path.GetExtension(httpPostedFile.FileName);

                if (fileExtension != ".xlsx")
                {
                    response.Message = "File format is not supported";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                if (httpPostedFile != null)
                {
                    try
                    {
                        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(httpPostedFile.InputStream);
                        response = UpdateUploadedFileData(excelReader);
                        excelReader.Close();
                    }
                    catch (Exception e)
                    {
                        response.Message = e.Message;
                    }
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private ResponseModel UpdateUploadedFileData(IExcelDataReader excelReader)
        {
            var response = new ResponseModel();
            DataSet result = excelReader.AsDataSet();
            DataTable Dt = result.Tables[0];

            for (int i = 1; i < Dt.Rows.Count; i++)
            {
                try
                {

                DataRow row = Dt.Rows[i];
                int columnCount = Dt.Columns.Count;
                string[] columns = new string[columnCount];
                for (int j = 0; j < columnCount; j++)
                {
                    if (Dt.Columns[j].DataType == typeof(Object))
                    {
                        try
                        {
                            columns[j] = Convert.ToDateTime(row[j]).ToString();
                        }
                        catch (Exception)
                        {
                            columns[j] = row[j].ToString();
                        }
                    }
                    else
                    {
                        columns[j] = row[j].ToString();
                    }
                    if (columns[0].Trim() == "EMP NO")
                        continue;

                    if (string.IsNullOrEmpty(columns[0].Trim()))
                        break;
                }
                if (string.IsNullOrEmpty(columns[0].Trim()))
                    break;

                response = _eBusiness.UpdateEmployeeBatchFile(columns,_userInfo.CompanyId);
                }
                catch (Exception ex)
                {

                }
            }
           
            return response;
        }
        [HttpPost]
        public ActionResult UploadEmployeeImage(int empId)
        {
            int? cId = _userInfo.CompanyId.HasValue && _userInfo.CompanyId.Value > 0 ? _userInfo.CompanyId.Value : (int?)null;
            var model = _eBusiness.GetEmployeeDetails(empId,cId);
            if (model == null)
                model = new EmployeeDetailsModel();
            var response = new ResponseModel();
            model.AttachedDocument = new LocalDocumentModel();
            if (!string.IsNullOrEmpty(model.ImageFileName))
            {
                string path = Server.MapPath(model.ImagePath);
                System.IO.File.Delete(path);
            }
            if (Request.Files.AllKeys.Any())
            {
                try
                {
                    model.AttachedDocument = UploadDriverImageFile();
                }
                catch (Exception exception)
                {
                    return Json(response = new ResponseModel { Success = false, Message = "Error in upload" });
                }
                _eBusiness.UpdateEmployeeImage(model);
            }
            return Json(new { FileName = model.AttachedDocument.UploadedFileName, FilePath = model.AttachedDocument.UploadedFileFullPath }, JsonRequestBehavior.AllowGet);
        }
        private LocalDocumentModel UploadDriverImageFile()
        {
            var respose = new LocalDocumentModel();
            try
            {
                if (Request.Files.AllKeys.Any())
                {
                    var httpPostedFile = Request.Files[0];
                    var fileName = httpPostedFile.FileName;
                    var fileExtension = Path.GetExtension(httpPostedFile.FileName);
                    var newFileName = Guid.NewGuid();
                    string blobName = newFileName + fileExtension;
                    string imagePath = Server.MapPath(Constants.LocalFilePath + blobName);
                    httpPostedFile.SaveAs(imagePath);
                    respose = new LocalDocumentModel
                    {
                        UploadedFileName = blobName,
                        UploadedFileFullPath = Constants.LocalFilePath + blobName,
                        DisplayFileName = fileName
                    };
                }
            }
            catch (Exception exception)
            {
            }
            return respose;
        }
    }
}
