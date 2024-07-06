using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ems.AttendanceTracking.Models;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;
using System.Web;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.Script.Serialization;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class CompanySettingsController : BaseController
    {
        private readonly ICompany _companyRepository;
        private readonly IUserCredential _userRepository;

        public CompanySettingsController()
        {
            _companyRepository = AttendanceUnityMapper.GetInstance<ICompany>();
            _userRepository = AttendanceUnityMapper.GetInstance<IUserCredential>();
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

            var query = _companyRepository.GetCompanyList().AsQueryable();
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
        public void SendMailToUser(string email, string loginID, string p)
        {
            if (string.IsNullOrEmpty(email))
                return;

            var sb = new StringBuilder();
            sb.Append(string.Format("Below is your portal login credential."));
            sb.Append(string.Format("<div></div>"));
            sb.Append(string.Format("<div>Your Login ID : {0}</div>", loginID));
            sb.Append(string.Format("<div>Your Password : {0}</div>", p));

            var recipient = new List<string> { email };
            new Email(ConfigurationManager.AppSettings["EmailSender"],
                ConfigurationManager.AppSettings["EmailSender"],
                "Your User Credential of EMS portal", sb.ToString())
                .SendEmail(recipient, ConfigurationManager.AppSettings["EmailSenderPassword"]);
        }


        [HttpGet]
        public JsonResult Get(int? id)
        {
            var model = _companyRepository.GetCompanyList().FirstOrDefault(x => x.Id == id);
            if (model == null)
                model = new Company();
            model.DocumentList = _companyRepository.GetCompanyAttachments(model.Id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            var response = _companyRepository.Delete(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadLogo()
        {
            try
            {
                var httpRequest = HttpContext.Request;
                var cId = httpRequest.Params["cId"];
                var httpFiles = httpRequest.Files;
                if (httpFiles.Count == 0)
                    return Json(new { Success = false, Message = "Files not found" }, JsonRequestBehavior.AllowGet);

                var postedFile = httpFiles[0];
                var fileExtension = Path.GetExtension(postedFile.FileName);
                var fileId = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                fileId = fileId + DateTime.Now.ToString("yymmssfff") + fileExtension;
                var picPath = HttpContext.Server.MapPath(@"~\UploadFiles\");
                if (!Directory.Exists(picPath))
                {
                    Directory.CreateDirectory(picPath);
                }
                if (!IsValidFile(fileExtension))
                    return Json(new { Success = true, Message = "File format is not valid" }, JsonRequestBehavior.AllowGet);

                var filePath = Path.Combine(picPath, fileId);
                Stream strm = postedFile.InputStream;
                Compressimage(strm, filePath, postedFile.FileName);

                _companyRepository.UpdateLogo(Convert.ToInt32(cId), fileId, postedFile.FileName);
                return Json(new { Success = true, FileId = fileId }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { Success = false, Message = string.Empty }, JsonRequestBehavior.AllowGet);
            }
        }

        public static void Compressimage(Stream sourcePath, string targetPath, String filename)
        {
            try
            {
                using (var image = Image.FromStream(sourcePath))
                {
                    float maxHeight = 1800.0f;
                    float maxWidth = 1800.0f;
                    int newWidth;
                    int newHeight;
                    string extension;
                    Bitmap originalBMP = new Bitmap(sourcePath);
                    int originalWidth = originalBMP.Width;
                    int originalHeight = originalBMP.Height;
                    if (originalWidth > maxWidth || originalHeight > maxHeight)
                    {
                        // To preserve the aspect ratio  
                        float ratioX = (float)maxWidth / (float)originalWidth;
                        float ratioY = (float)maxHeight / (float)originalHeight;
                        float ratio = Math.Min(ratioX, ratioY);
                        newWidth = (int)(originalWidth * ratio);
                        newHeight = (int)(originalHeight * ratio);
                    }
                    else
                    {
                        newWidth = (int)originalWidth;
                        newHeight = (int)originalHeight;

                    }
                    Bitmap bitMAP1 = new Bitmap(originalBMP, newWidth, newHeight);
                    Graphics imgGraph = Graphics.FromImage(bitMAP1);
                    extension = Path.GetExtension(targetPath);
                    if (extension == ".png" || extension == ".gif")
                    {
                        imgGraph.SmoothingMode = SmoothingMode.AntiAlias;
                        imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);


                        bitMAP1.Save(targetPath, image.RawFormat);

                        bitMAP1.Dispose();
                        imgGraph.Dispose();
                        originalBMP.Dispose();
                    }
                    else if (extension == ".jpg")
                    {

                        imgGraph.SmoothingMode = SmoothingMode.AntiAlias;
                        imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        bitMAP1.Save(targetPath, jpgEncoder, myEncoderParameters);

                        bitMAP1.Dispose();
                        imgGraph.Dispose();
                        originalBMP.Dispose();

                    }


                }

            }
            catch (Exception)
            {
                throw;

            }
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private bool IsValidFile(string ext)
        {
            string[] validFileFormate = new string[] { "jpg", "png", "gif" };
            for (int i = 0; i < validFileFormate.Length; i++)
            {
                string vF = "." + validFileFormate[i];
                if (vF == ext)
                {
                    return true;
                }
            }
            return false;
        }
        [HttpPost]
        public JsonResult AddOrUpdateCompany(string jsonString)
        {
            var respone = new ResponseModel();

            var model = new JavaScriptSerializer().Deserialize<Company>(jsonString);
            try
            {
                if (model.Id > 0)
                {
                    DeleteCompanyAttachments(model.DocumentList);
                }
                else
                {
                    model.CompanyAdminPassword = CryptographyHelper.CreateMD5Hash(model.AdminAssignedPassword);
                }
                CompanyAttachedDocuments(model);
            }
            catch (Exception exception)
            {
                return Json(new ResponseModel { Message = "Error in upload" });
            }

            model.CreatedById = _userInfo.Id;
            
            ConvertStringToDateTime(model);
            var userEntity = _userRepository.GetByLoginID(model.CompanyAdminLoginID);
            if (userEntity != null && model.Id==0)
                return Json(new ResponseModel { Message="Sorry! This Login ID already exists."});
            respone = _companyRepository.AddOrUpdateCompany(model);
           
            if (respone.Success && model.Id == 0)
                SendMailToUser(model.CompanyAdminEmail, model.CompanyAdminLoginID, model.AdminAssignedPassword);
            return Json(respone);
        }
        private void ConvertStringToDateTime(Company model)
        {
            model.CompanyRegistrationExpiryDate = string.IsNullOrEmpty(model.CompanyRegistrationExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.CompanyRegistrationExpiryDateVw);
            model.EstablishmentCardExpiryDate = string.IsNullOrEmpty(model.EstablishmentCardExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.EstablishmentCardExpiryDateVw);
            model.TradeLicenseExpiryDate = string.IsNullOrEmpty(model.TradeLicenseExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.TradeLicenseExpiryDateVw);
            model.OthersExpiryDate = string.IsNullOrEmpty(model.OthersExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.OthersExpiryDateVw);
        }
        private void DeleteCompanyAttachments(List<AttachmentModel> list)
        {
            if (list == null || list.Count <= 0)
                return;

            foreach (var model in list)
            {
                if (!model.IsDeleted)
                    continue;

                DeleteCompanyAttachement(model);

            }
        }
        [HttpPost]
        public ActionResult DeleteAttachedFile(List<AttachmentModel> model)
        {
            foreach(var item in model)
            {
                if (!item.IsDeleted)
                    continue;
                if (!string.IsNullOrEmpty(item.BlobName))
                {
                    DeleteCompanyAttachement(item);
                }
            }
            return Json(new ResponseModel { Success = true, Message = "Deleted successfully" });
        }
        private void DeleteCompanyAttachement(AttachmentModel model)
        {
            try
            {
                string path = Server.MapPath(model.UploadedFileFullPath);
                System.IO.File.Delete(path);
                _companyRepository.DeleteCompanyAttachments(model.Id);
            }
            catch (Exception exception)
            {

            }
        }
        private void CompanyAttachedDocuments(Company model)
        {
            if (Request.Files.AllKeys.Any())
            {
                model.AttachedDocumentList = new List<AttachmentModel>();
                int i = 0;
                foreach (var key in Request.Files.AllKeys)
                {
                    var httpPostedFile = Request.Files[i++];
                    int attachmentType = 0;

                    if (key.Contains("CompanyRegistration"))
                    {
                        attachmentType = (int)CompanyAttachmentType.CompanyRegistration;
                    }
                    else if (key.Contains("EstablishmentCard"))
                    {
                        attachmentType = (int)CompanyAttachmentType.EstablishmentCard;
                    }
                    else if (key.Contains("TradeAndMunicipalLicense"))
                    {
                        attachmentType = (int)CompanyAttachmentType.TradeAndMunicipalLicense;
                    }
                    else if (key.Contains("Others"))
                    {
                        attachmentType = (int)CompanyAttachmentType.Others;
                    }
                    if (attachmentType > 0)
                        model.AttachedDocumentList.Add(UploadRequestDocuments(httpPostedFile, attachmentType));
                }
            }
        }
        private AttachmentModel UploadRequestDocuments(HttpPostedFileBase httpPostedFile, int? attachmentType)
        {
            var model = new AttachmentModel();

            var fileName = httpPostedFile.FileName;
            var fileExtension = Path.GetExtension(httpPostedFile.FileName);
            var newFileName = Guid.NewGuid();
            string blobName = newFileName + fileExtension;
            string imagePath = Server.MapPath(Constants.LocalFilePath + blobName);
            httpPostedFile.SaveAs(imagePath);

            model = new AttachmentModel
            {
                AttachmentTypeId = attachmentType.HasValue ? attachmentType.Value : (int?)null,
                FileName = fileName,
                BlobName = blobName,
            };
            return model;
        }
    }
}
