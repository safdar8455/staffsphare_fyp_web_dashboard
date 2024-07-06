using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;
namespace Ems.AttendanceTracking.Services
{
	public class TaskBusiness
	{
		private readonly ITask _TaskBusiness;

		public TaskBusiness()
		{
			_TaskBusiness = AttendanceUnityMapper.GetInstance<ITask>();
		}
		public ResponseModel AddOrUpdate(TaskModel model)
		{
			model.DueDate= string.IsNullOrEmpty(model.DueDateStr) ? (DateTime?)null : Convert.ToDateTime(model.DueDateStr);
			return _TaskBusiness.AddOrUpdate(model,model.UplodedDocumentList);
		}
		public IEnumerable<TaskModel> GetAll(int? cId)
		{
			var list = _TaskBusiness.GetAll(cId);
			return list;
		}
		public IEnumerable<TaskModel> GetCalender(int? cId)
		{
			var list = _TaskBusiness.GetAll(cId);
			return list;
		}
		public IEnumerable<TaskModel> GetAllTaskTypeFilter(int? cId,int? TaskTypeId)
		{
            if (TaskTypeId == null)
            {
				var list = _TaskBusiness.GetAll(cId);
				return list;
			}
            else
            {
				var list = _TaskBusiness.GetAll(cId).Where(x => x.TaskTypeId == TaskTypeId);
				return list;
			}
			
		}
		public IEnumerable<TaskModel> GetAll(int? cId, int? StatusId, int? DateTypeId)
		{
			if (DateTypeId == null && StatusId==null)
			{
				var list = _TaskBusiness.GetAll(cId);
				return list;
			}
			if (DateTypeId == null)
            {
				var list = _TaskBusiness.GetAll(cId).Where(x => x.StatusId == StatusId);
				return list;
            }
            else
            {
				DateTime StartDate = DateTime.Today;
                if (DateTypeId == 1)
                {
					var list = _TaskBusiness.GetAllDateFilter(cId, StartDate, StartDate);
					return list;
				}
                else if(DateTypeId == 2)
                {
					DateTime EndDate = StartDate.AddDays(-3);
					var list = _TaskBusiness.GetAllDateFilter(cId, StartDate, EndDate);
					return list;
				}
                else
                {
					DateTime EndDate = StartDate.AddDays(-7);
					var list = _TaskBusiness.GetAllDateFilter(cId, StartDate, EndDate);
					return list;
				}
				
			}
			
		}
		public TaskModel GetById(string id,int? companyId)
		{
			var model = _TaskBusiness.GetAll(companyId).Where(x=>x.Id==id).FirstOrDefault();
			if (model == null)
				model = new TaskModel();
			model.TaskDocumentList = _TaskBusiness.GetTaskAttachments(id);
			if (model.TaskDocumentList == null)
				model.TaskDocumentList = new List<TaskAttachment>();
			model.EmpAssignedList = _TaskBusiness.GetEmployeeAssignedTextValue(id);
			if (model.EmpAssignedList == null)
				model.EmpAssignedList = new List<TextValuePairModelEmp>();
			LoadTaskDropDownData(model,companyId);
			return model;
		}
		public TaskModel Getddl(int? companyId)
		{
			var	model = new TaskModel();
			LoadTaskDropDownData(model, companyId);
			return model;
		}
		public ResponseModel DeleteAttachment(string id)
		{
			return _TaskBusiness.DeleteAttachment(id);
		}
	
		private void LoadTaskDropDownData(TaskModel model,int? companyId)
		{
			model.StatusList = Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>().Select(c => new NameIdPairModel
			{
				Id = (int)c,
				Name = EnumUtility.GetDescriptionFromEnumValue(c)
			}).ToList();
			model.PriorityList = Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>().Select(c => new NameIdPairModel
			{
				Id = (int)c,
				Name = EnumUtility.GetDescriptionFromEnumValue(c)
			}).ToList();
			model.TaskType = Enum.GetValues(typeof(TaskType)).Cast<TaskType>().Select(c => new NameIdPairModel
			{
				Id = (int)c,
				Name = EnumUtility.GetDescriptionFromEnumValue(c)
			}).ToList();
			model.DateFilterTypeList = Enum.GetValues(typeof(DateFilterType)).Cast<DateFilterType>().Select(c => new NameIdPairModel
			{
				Id = (int)c,
				Name = EnumUtility.GetDescriptionFromEnumValue(c)
			}).ToList();
			model.AssignedList = _TaskBusiness.GetEmployeeAsTextValue(companyId??0);
			model.MultiAssignedList = _TaskBusiness.GetEmployeeDDLAsTextValue(companyId ?? 0);


			
		}
		public ResponseModel Delete(string id)
		{
			return _TaskBusiness.Delete(id);
		}
	}
}
