using Ems.BusinessTracker.Common;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;
using Webclient.Controllers.Api;


namespace Webclient.Areas.AttendanceTracking.Controllers.Api
{
    public class NoticeApiController : BaseApiController
    {
        private readonly INoticeBoard _notice;
        public NoticeApiController()
        {
            _notice = AttendanceUnityMapper.GetInstance<INoticeBoard>();
        }


        [HttpPost]
        public IHttpActionResult Upload()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    HttpFileCollection fileCollection;
                    HttpPostedFile postedFile;
                    Stream fileStream;

                    fileCollection = httpRequest.Files;
                    postedFile = fileCollection[0];

                    fileStream = postedFile.InputStream;

                    var fileType = Path.GetExtension(postedFile.FileName);

                    string blobName = Guid.NewGuid() + fileType;

                    string imagePath = System.Web.Hosting.HostingEnvironment.MapPath(Constants.LocalFilePath + blobName);
                    postedFile.SaveAs(imagePath);

                    return Ok(new { Success = true, Message = string.Empty, ReturnCode = blobName });
                }
            }
            catch (Exception exception)
            {
                return Ok(new { Success = false, Message = exception.Message });
            }
            return Ok(new { Success = false, Message = string.Empty });
        }


        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            var result = _notice.GetAll(this.CompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            var result = _notice.Get(id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        /// <summary>
        /// Notice save for only text or image also
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Save(JObject jObject)
        {
            dynamic json = jObject;
            var noticeBoard = new NoticeBoardModel
            {
                Details = json.Details,
                ImageFileName = json.ImageFileName,
                CreatedBy = this.UserId,
                CompanyId=this.CompanyId
            };

            var response = _notice.Save(noticeBoard);
            return Ok(response);

        }
    }
}
