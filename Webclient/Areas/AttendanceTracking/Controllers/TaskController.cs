using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ems.AttendanceTracking.Models;
using Ems.AttendanceTracking.Services;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class TaskController : BaseController
    {
        public TaskController()
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
        public JsonResult GetAll(GridSettings grid, int? StatusId, int? DateTypeId)
        {

            var query = new TaskBusiness().GetAll(this._userInfo.CompanyId, StatusId, DateTypeId).AsQueryable();
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
        [HttpGet]
        public JsonResult GetAllWithOutFilter()
        {

            var query = new TaskBusiness().GetAll(this._userInfo.CompanyId).AsQueryable();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCalenderTaskList()
        {

            var query = new TaskBusiness().GetCalender(this._userInfo.CompanyId).AsQueryable();
            List<TaskCal> events = new List<TaskCal>();
            foreach (var item in query)
            {
                TaskCal events1 = new TaskCal();
                events1.Id = item.Id;
                events1.title = item.Title;
                events1.start = item.CreatedAt.HasValue? item.CreatedAt.Value.ToString(Constants.DateFormat) : string.Empty;
                events1.color = "tomato";
                events1.textColor = "white";
                events.Add(events1);
            }
            return Json(events, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllWithFilter(GridSettings grid, int? StatusId, int? DateTypeId)
        {

            var query = new TaskBusiness().GetAll(this._userInfo.CompanyId, StatusId, DateTypeId).AsQueryable();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllTaskTypeFilter(GridSettings grid, int? TaskTypeId)
        {

            var query = new TaskBusiness().GetAllTaskTypeFilter(this._userInfo.CompanyId, TaskTypeId).AsQueryable();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveTask(string jsonString)
        {
            var model = new JavaScriptSerializer().Deserialize<TaskModel>(jsonString);
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            try
            {
                AttacheRequestDocuments(model);
            }
            catch (Exception exception)
            {
                return Json(new ResponseModel { Message = "Error in upload" });
            }
            model.CompanyId = this._userInfo.CompanyId;
            model.CreatedById = this._userInfo.Id;
            var response = new TaskBusiness().AddOrUpdate(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteAttachedFile(TaskAttachment model)
        {
            if (!string.IsNullOrEmpty(model.BlobName))
            {
               DeleteTaskAttachement(model);
            }
            return Json(new ResponseModel { Success = true, Message = "Deleted successfully" });
        }
        private void AttacheRequestDocuments(TaskModel model)
        {
            if (Request.Files.AllKeys.Any())
            {
                model.UplodedDocumentList = new List<TaskAttachment>();
                foreach (var key in Request.Files.AllKeys)
                {
                    var httpPostedFile = Request.Files[key];
                    model.UplodedDocumentList.Add(UploadRequestDocuments(httpPostedFile));

                }
            }
        }
        private TaskAttachment UploadRequestDocuments(HttpPostedFileBase httpPostedFile)
        {
            var model = new TaskAttachment();
            Stream myStream;
            var fileLength = httpPostedFile.ContentLength;
            byte[] input = new byte[fileLength];

            // Initialize the stream.
            myStream = httpPostedFile.InputStream;
            var fileName = httpPostedFile.FileName;
            var fileExtension = Path.GetExtension(httpPostedFile.FileName);
            var newFileName = Guid.NewGuid();
            string blobName = newFileName + fileExtension;
            string imagePath = Server.MapPath(Constants.LocalFilePath + blobName);
            httpPostedFile.SaveAs(imagePath);
            model = new TaskAttachment
            {
                FileName = fileName,
                BlobName = blobName,
            };
            return model;
        }
        [HttpGet]
        public JsonResult Get(string id)
        {
            var result = new TaskBusiness().GetById(id,_userInfo.CompanyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Getddl()
        {
            var result = new TaskBusiness().Getddl( _userInfo.CompanyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private void DeleteTaskAttachement(TaskAttachment model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.BlobName))
                {
                    string path = Server.MapPath(model.FullPath);
                    System.IO.File.Delete(path);
                }
                new TaskBusiness().DeleteAttachment(model.Id);
            }
            catch (Exception exception)
            {
                
            }
        }
        [HttpGet]
        public JsonResult Delete(string Id)
        {
            var task = new TaskBusiness().GetById(Id,_userInfo.CompanyId);
            if(task.TaskDocumentList.Count()>0)
            {
                foreach(var item in task.TaskDocumentList)
                {
                    DeleteTaskAttachement(item);
                }
            }
            var response = new TaskBusiness().Delete(Id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}
