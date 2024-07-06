using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;
using Webclient.Controllers.Api;


namespace Webclient.Areas.AttendanceTracking.Controllers.Api
{
    public class TaskApiController : BaseApiController
    {
        private readonly ITask _task;
        public TaskApiController()
        {
            _task = AttendanceUnityMapper.GetInstance<ITask>();
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


        [HttpPost]
        public IHttpActionResult SaveTask()
        {
            var httpRequest = HttpContext.Current.Request;
            var modelst = httpRequest.Params["taskmodel"];
            var model = JsonConvert.DeserializeObject<TaskModel>(modelst);
            var taskAttachmentsModelst = httpRequest.Params["taskAttachmentsModel"];
            var taskAttachmentsModelOb = JsonConvert.DeserializeObject<List<TaskAttachment>>(taskAttachmentsModelst);
            if (!model.StatusId.HasValue)
            {
                model.StatusId = (int)TaskStatus.ToDo;
            }
            if (!model.PriorityId.HasValue)
            {
                model.PriorityId = (int)TaskPriority.Low;
            }

            var result = _task.AddOrUpdate(model, taskAttachmentsModelOb);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetTaskAttachments(string taskId)
        {
            var result = _task.GetTaskAttachments(taskId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetCreatedByMeTasks(string userId)
        {
            var result = _task.GetTaskList(new TaskModel { CreatedById = userId }).OrderByDescending(x => x.CreatedAt);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetTasksWithCompany(string companyId)
        {
            var result = _task.GetTaskList(new TaskModel { CompanyId = Convert.ToInt32(companyId) }).OrderByDescending(x => x.CreatedAt);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetAssignedToMeTasks(string userId)
        {
            var result = _task.GetTaskList(new TaskModel { AssignedTold = userId }).OrderByDescending(x => x.CreatedAt);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetRelatedToMeTasks(string userId)
        {
            var result = _task.GetAll(this.CompanyId);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetTasksByCompanyId(string companyId)
        {
            var result = _task.GetTasksByCompanyId(companyId).OrderByDescending(x => x.CreatedAt);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetCreatedByMeTasksWithCompany(string userId, string companyId)
        {
            var result = _task.GetTaskList(new TaskModel { CreatedById = userId }).OrderByDescending(x => x.CreatedAt).Where(r => r.CompanyId == Convert.ToInt32(companyId)).ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAssignedToMeTasksWithCompany(string userId, string companyId)
        {
            var result = _task.GetTaskList(new TaskModel { AssignedTold = userId }).OrderByDescending(x => x.CreatedAt).Where(r => r.CompanyId == Convert.ToInt32(companyId)).ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetRelatedToMeTasksWithCompany(string userId, string companyId)
        {
            var result = _task.GetRelatedToMeTaskList(new TaskModel { AssignedTold = userId }).OrderByDescending(x => x.CreatedAt).Where(r => r.CompanyId == Convert.ToInt32(companyId)).ToList();
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult DeleteTask(string id)
        {
            var result = _task.DeleteTask(id);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult GetTaskStatusList()
        {
            var list = Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>().Select(v => new NameIdPairModel
            {
                Name = EnumUtility.GetDescriptionFromEnumValue(v),
                Id = (int)v
            }).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IHttpActionResult GetPriorityList()
        {
            var list = Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>().Select(v => new NameIdPairModel
            {
                Name = EnumUtility.GetDescriptionFromEnumValue(v),
                Id = (int)v
            }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IHttpActionResult GetEmployeeAsTextValue()
        {
            var userResponse = _task.GetEmployeeAsTextValue(this.CompanyId);
            return Ok(userResponse);
        }
        [HttpGet]
        public IHttpActionResult GetAllAssignTo(string taskId)
        {
            var result = _task.GetEmployeeAssignedTextValue(taskId);
            return Ok(result);
        }
    }
}
