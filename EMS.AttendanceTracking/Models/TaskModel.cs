using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;
using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;

namespace Ems.AttendanceTracking.Models
{
    public class TaskModel
    {
        public string Id { get; set; }
        public int? TaskNo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedById { get; set; }
        [ScriptIgnore]
        public DateTime? CreatedAt { get; set; }
        public string AssignedTold { get; set; }
        public List<IdList> AssignedToldList { get; set; }
        public string ContractNumber { get; set; }
        public string AssignToName { get; set; }
        public string UpdatedById { get; set; }
        public int? StatusId { get; set; }
        public int? TaskTypeId { get; set; }
        public string OutDoorAddr { get; set; }
        public decimal? OurDoorLat { get; set; }
        public decimal? OurDoorLong { get; set; }

        public int? TaskGroupId { get; set; }
        [ScriptIgnore]
        public DateTime? DueDate { get; set; }
        public int? CompanyId { get; set; }
        public int? PriorityId { get; set; }
        public string UpdatedByName { get; set; }
        [ScriptIgnore]
        public DateTime? UpdatedAt { get; set; }
        public string DueDateStr { get; set; }
        public string DueDateVw
        {
            get { return DueDate.HasValue ? DueDate.Value.ToString(Constants.DateLongFormat) : string.Empty; }
        }

        public string CreatedAtVw
        {
            get { return CreatedAt.HasValue ? CreatedAt.Value.ToString(Constants.DateTimeLongFormat) : string.Empty; }
        }

        public class pushModel
        {
            public string[] poshList { get; set; }
        }

        public string UpdatedAtVw
        {
            get { return UpdatedAt.HasValue ? UpdatedAt.Value.ToString(Constants.DateTimeLongFormat) : string.Empty; }
        }

        public string StatusName
        {
            get
            {
                if (!StatusId.HasValue)
                    return string.Empty;
                return EnumUtility.GetDescriptionFromEnumValue((TaskStatus)StatusId);
            }
        }
        public string TaskTypeName
        {
            get
            {
                if (!TaskTypeId.HasValue)
                    return string.Empty;
                return EnumUtility.GetDescriptionFromEnumValue((TaskType)TaskTypeId);
            }
        }

        public string PriorityName
        {
            get
            {
                if (!PriorityId.HasValue)
                {
                    PriorityId = (int)TaskPriority.Medium;
                }
                return EnumUtility.GetDescriptionFromEnumValue((TaskPriority)PriorityId);
            }
        }
       

        public bool HasAttachments
        {
            get { return false; }
        }
        public string CreatedByName { get; set; }
        public bool CanDelete { get; set; }
        public List<NameIdPairModel> PriorityList { get; set; }
        public List<NameIdPairModel> TaskType { get; set; }
        public List<NameIdPairModel> DateFilterTypeList { get; set; }

        public List<NameIdPairModel> StatusList { get; set; }
        public List<TextValuePairModel> AssignedList { get; set; }
        public List<TextValuePairModelEmp> MultiAssignedList { get; set; }
        public List<TextValuePairModelEmp> EmpAssignedList { get; set; }
        public List<TaskAttachment> TaskDocumentList { get; set; }
        public List<TaskAttachment> UplodedDocumentList { get; set; }
    }
    public class IdList
    {
        public string id { get; set; }
    }
    public class TasKAssignTo
    {
        public string Id { get; set; }
        public string TaskId { get; set; }
        public string AssignedToId { get; set; }
    }
    public class TaskAttachment
    {
        public string Id { get; set; }
        public string TaskId { get; set; }
        public string FileName { get; set; }
        public string BlobName { get; set; }
        public string FullPath { get; set; }
        public bool IsNew { get; set; }
        public string UpdatedById { get; set; }
        [ScriptIgnore]
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedAtVw
        {
            get { return UpdatedAt.HasValue ? UpdatedAt.Value.ToString(Constants.DateTimeLongFormat) : string.Empty; }
        }
    }
    public class TaskCal
    {
        public string Id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }


    }
   
    public enum TaskStatus
    {
        [Description("To Do")]
        ToDo = 1,
        [Description("Pause")]
        Pause = 2,
        [Description("Done")]
        Done = 3,
        [Description("Redo")]
        Redo = 4,
        [Description("Cancelled")]
        Cancelled = 5

    }

    public enum TaskPriority
    {
        [Description("High")]
        High = 1,
        [Description("Medium")]
        Medium = 2,
        [Description("Low")]
        Low = 3
    }
    public enum TaskType
    {
        [Description("Indoor")]
        Indoor = 1,
        [Description("Outdoor")]
        Outdoor = 2,      
    }
    public enum DateFilterType
    {
        [Description("Today")]
        Today = 1,
        [Description("Last 3 Days")]
        Last_3_Days = 2,
        [Description("Last 7 Days")]
        Last_7_Days = 3,
    }

}
