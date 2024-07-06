using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
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
    public class LeaveApiController : BaseApiController
    {
        private readonly ILeave _leave;
        private readonly ILeavePolicy _LeavePolicy;
        public LeaveApiController()
        {
            _leave = AttendanceUnityMapper.GetInstance<ILeave>();
            _LeavePolicy = AttendanceUnityMapper.GetInstance<ILeavePolicy>();
        }


        [HttpPost]
        public IHttpActionResult UploadDocuments()
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
        public HttpResponseMessage GetLeaveByCompanyId()
        {
            var result = _leave.GetLeaveByCompanyId(this.CompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetUserLeaves(string userId)
        {
            var result = _leave.GetUserLeaves(userId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [HttpGet]
        public HttpResponseMessage GetUserPendingLeaves(string userId)
        {
            var result = _leave.GetUserPendingLeaves(userId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public IHttpActionResult CreateLeave(JObject jObject)
        {
            dynamic json = jObject;
            var model = new LeaveModel
            {
                CompanyId = this.CompanyId,
                Id=json.Id,
                FromDate = Convert.ToDateTime(json.LeaveApplyFrom),
                ToDate = Convert.ToDateTime(json.LeaveApplyTo),
                IsHalfDay = json.IsHalfDay,
                LeaveTypeId = json.LeaveTypeId,
                LeaveReason = json.LeaveReason,
                CreatedAt = DateTime.Now.ToString(),
                IsApproved = false,
                IsRejected = false,
                IsCorrection=false,
                RejectReason = json.RejectReason,
                ApprovedById = null,
                ApprovedAt = null,
                UserId = json.UserId,
                StatusId=(int)LeaveStatus.Pending,
            };
            var response = _leave.CreateEmployeeLeave(model);
            if (!response.Success)
                return Ok(response);
            return Ok(new { Success = true });
        }

        [HttpPost]
        public IHttpActionResult ApproveOrRejectLeave(LeaveModel model)
        {
            var leaveModel = new LeaveApproveModel
            {
                RequestNo = model.RequestNo,
                SerialNo = model.ApproverSerialId,
                Approved = model.IsApproved,
                Rejected = model.IsRejected,
                RejectReason = model.RejectReason,
                ApproverId = this.UserId,
                UpdatedById = this.UserId,
                UpdatedAt = DateTime.UtcNow
            };
            var response = _leave.ApproveOrRejectLeave(leaveModel);
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult Approved(int id)
        {
            var response = _leave.Approved(id,this.UserId);
            return Ok(response);
        }
        [HttpGet]
        public IHttpActionResult Rejected(int id)
        {
            var response = _leave.Rejected(id);
            return Ok(response);
        }
        [HttpGet]
        public IHttpActionResult Correction(int id)
        {
            var response = _leave.Correction(id);
            return Ok(response);
        }
        [HttpGet]
        public IHttpActionResult GetLeaveTypeList()
        {
            var response = _LeavePolicy.GetAll();
            var list = response.Select(v => new NameIdPairModel
            {
                Name = v.Description,
                Id = v.Id
            }).ToList();
            return Ok(list);
        }
    }
}
