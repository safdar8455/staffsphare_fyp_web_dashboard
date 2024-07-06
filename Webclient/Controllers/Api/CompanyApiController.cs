using Ems.BusinessTracker.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Webclient.Controllers.Api
{
    public class CompanyApiController : BaseApiController
    {
        private readonly ICompany _companyRepository;
        private readonly IUserCredential _userCredential;
        public CompanyApiController()
        {
            _companyRepository = AttendanceUnityMapper.GetInstance<ICompany>();
            _userCredential = AttendanceUnityMapper.GetInstance<IUserCredential>();
        }

        [HttpGet]
        public HttpResponseMessage GetMenuList()
        {
            var userProfile = _userCredential.GetProfileDetails(this.UserId);
            var result = MenuCollection.GetMenu(userProfile.UserTypeId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        [HttpGet]
        public HttpResponseMessage GetCompanyList()
        {
            var result = _companyRepository.GetCompanyList();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [HttpGet]
        public HttpResponseMessage DeleteCompany(int id)
        {
            var result = _companyRepository.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public IHttpActionResult Save([FromBody]Company json)
        {
            json.CreatedById = this.UserId;
            json.CompanyAdminPassword = CryptographyHelper.CreateMD5Hash(json.AdminAssignedPassword);
            var response = _companyRepository.Create(json);
            if (response.Success)
                SendMailToUser(json.CompanyAdminEmail, json.CompanyAdminLoginID, json.AdminAssignedPassword);
            return Ok(response);
        }

        [HttpPost]
        public IHttpActionResult Update([FromBody]Company json)
        {
            var response = _companyRepository.Update(json);
            return Ok(response);
        }

      
        public void SendMailToUser(string email, string loginID, string p)
        {
            if (string.IsNullOrEmpty(email))
                return;

            var sb = new StringBuilder();
            sb.Append(string.Format("Below is your portal login credential.You can login to EMS portal."));
            sb.Append(string.Format("<div></div>"));
            sb.Append(string.Format("<div>Your Login ID : {0}</div>", loginID));
            sb.Append(string.Format("<div>Your Password : {0}</div>", p));

            var recipient = new List<string> { email };
            new Email(ConfigurationManager.AppSettings["EmailSender"],
                ConfigurationManager.AppSettings["EmailSender"],
                "Your User Credential", sb.ToString())
                .SendEmail(recipient, ConfigurationManager.AppSettings["EmailSenderPassword"]);
        }


        [HttpPost]
        public HttpResponseMessage UploadLogo()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var cId = httpRequest.Params["cId"];
                var httpFiles = httpRequest.Files;
                if (httpFiles.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.OK, new { Success = false, Message = "Files not found" });

                var postedFile = httpFiles[0];
                var fileExtension = Path.GetExtension(postedFile.FileName);
                var fileId = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                fileId = fileId + DateTime.Now.ToString("yymmssfff") + fileExtension;
                var picPath = HttpContext.Current.Server.MapPath(@"~\UploadFiles\");
                if (!Directory.Exists(picPath))
                {
                    Directory.CreateDirectory(picPath);
                }
                if (!IsValidFile(fileExtension))
                    return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "File format is not valid"});

                var filePath = Path.Combine(picPath, fileId);
                Stream strm = postedFile.InputStream;
                Compressimage(strm, filePath, postedFile.FileName);

                _companyRepository.UpdateLogo(Convert.ToInt32(cId), fileId, postedFile.FileName);
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, FileId = fileId });
            }
            catch (System.Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = false, Message = string.Empty });
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
            string[] validFileFormate = new string[] { "jpg", "png", "gif"};
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
    }

}
