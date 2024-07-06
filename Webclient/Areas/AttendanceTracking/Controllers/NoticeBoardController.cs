using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
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
    public class NoticeBoardController : BaseController
    {
        public NoticeBoardController()
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
            var query = new NoticeBoardBusiness().GetAll(_userInfo.CompanyId??0).AsQueryable();
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
        public ActionResult Save(NoticeBoardModel model)
        {
            model.CompanyId = this._userInfo.CompanyId;
            model.CreatedBy = this._userInfo.Id;
            return Json(new NoticeBoardBusiness().Save(model));
        }
        [HttpGet]
        public JsonResult Get(string id)
        {
            return Json(new NoticeBoardBusiness().Get(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Delete(string Id)
        {
            var response = new NoticeBoardBusiness().Delete(Id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadNoticeImage(string noticeId)
        {
            var model = new NoticeBoardBusiness().Get(noticeId);
            if (model == null)
                model = new NoticeBoardModel();
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
                    model.AttachedDocument = UploadImageFile();
                }
                catch (Exception exception)
                {
                    return Json(response = new ResponseModel { Success = false, Message = "Error in upload" });
                }
                new NoticeBoardBusiness().UploadImageFile(model);
            }
            return Json(new { FileName = model.AttachedDocument.UploadedFileName, FilePath = model.AttachedDocument.UploadedFileFullPath }, JsonRequestBehavior.AllowGet);
        }
        private LocalDocumentModel UploadImageFile()
        {
            var respose = new LocalDocumentModel();
            try
            {
                if (Request.Files.AllKeys.Any())
                {
                    var httpPostedFiles = Request.Files;
                    Stream myStream;
                    var httpPostedFile = Request.Files[0];
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
