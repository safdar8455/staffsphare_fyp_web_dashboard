using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;
using System;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface ITask
    {
        List<TaskModel> GetAll(int? cId);
        ResponseModel Delete(string id);
        ResponseModel AddOrUpdate(TaskModel model, List<TaskAttachment> attachmentsModel);
        List<TaskAttachment> GetTaskAttachments(string taskId);
        List<TaskModel> GetTaskList(TaskModel sModel);
        List<TaskModel> GetAssignedToMeTasks(TaskModel sModel);
        List<TaskModel> GetRelatedToMeTaskList(TaskModel sModel);
        List<TaskModel> GetTasksByCompanyId(string companyId);
        ResponseModel DeleteTask(string id);
        ResponseModel DeleteAttachment(string id); 
        List<TextValuePairModel> GetEmployeeAsTextValue(int companyId);
        List<TextValuePairModelEmp> GetEmployeeDDLAsTextValue(int companyId);
        List<TaskModel> GetAllDateFilter(int? cId, DateTime StartDate, DateTime EndDate);
        List<TextValuePairModelEmp> GetEmployeeAssignedTextValue(string id);

    }
}
