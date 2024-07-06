using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Ems.AttendanceTracking.Models;
using Ems.BusinessTracker.Common;

namespace Ems.AttendanceTracking.Mappers
{
    public static class TaskMapper
    {  
        public static List<TaskAttachment> ToTaskAttachment(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<TaskAttachment>();

            while (readers.Read())
            {
                var model = new TaskAttachment
                {
                    Id = Convert.ToString(readers["Id"]),
                    TaskId = Convert.IsDBNull(readers["TaskId"]) ? string.Empty : Convert.ToString(readers["TaskId"]),
                    FileName = Convert.IsDBNull(readers["FileName"]) ? string.Empty : Convert.ToString(readers["FileName"]),
                    BlobName = Convert.IsDBNull(readers["BlobName"]) ? string.Empty : Convert.ToString(readers["BlobName"]),
                    UpdatedAt = Convert.IsDBNull(readers["UpdatedAt"]) ? (DateTime?)null : Convert.ToDateTime(readers["UpdatedAt"]),
                    UpdatedById = Convert.IsDBNull(readers["UpdatedById"]) ? string.Empty : Convert.ToString(readers["UpdatedById"])
                };
                model.FullPath = Constants.LocalFilePath + model.BlobName;
                models.Add(model);
            }

            return models;
        }
        public static List<TaskModel> ToTask(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<TaskModel>();

            while (readers.Read())
            {
                var model = new TaskModel
                {
                    Id = Convert.ToString(readers["Id"]),
                    Title = Convert.IsDBNull(readers["Title"]) ? string.Empty : Convert.ToString(readers["Title"]),
                    Description = Convert.IsDBNull(readers["Description"]) ? string.Empty : Convert.ToString(readers["Description"]),
                    StatusId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                    CreatedById = Convert.IsDBNull(readers["CreatedById"]) ? string.Empty : Convert.ToString(readers["CreatedById"]),
                    DueDate = Convert.IsDBNull(readers["DueDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["DueDate"]),
                    PriorityId = Convert.IsDBNull(readers["PriorityId"]) ? (int?)null : Convert.ToInt32(readers["PriorityId"]),
                    CreatedAt = Convert.IsDBNull(readers["CreatedAt"]) ? (DateTime?)null : Convert.ToDateTime(readers["CreatedAt"]),
                    CreatedByName = Convert.IsDBNull(readers["CreatedByName"]) ? string.Empty : Convert.ToString(readers["CreatedByName"]),
                    AssignToName = Convert.IsDBNull(readers["AssignToName"]) ? string.Empty : Convert.ToString(readers["AssignToName"]),
                    AssignedTold = Convert.IsDBNull(readers["AssignedToId"]) ? string.Empty : Convert.ToString(readers["AssignedToId"]),
                    TaskTypeId = Convert.IsDBNull(readers["TaskTypeId"]) ? (int?)null : Convert.ToInt32(readers["TaskTypeId"]),
                    ContractNumber = Convert.IsDBNull(readers["ContractNumber"]) ? string.Empty : Convert.ToString(readers["ContractNumber"]),
                    OutDoorAddr = Convert.IsDBNull(readers["OutDoorAddr"]) ? string.Empty : Convert.ToString(readers["OutDoorAddr"]),
                    OurDoorLat = Convert.IsDBNull(readers["OurDoorLat"]) ? (decimal?)null : Convert.ToDecimal(readers["OurDoorLat"]),
                    OurDoorLong = Convert.IsDBNull(readers["OurDoorLong"]) ? (decimal?)null : Convert.ToDecimal(readers["OurDoorLong"])
                };

                models.Add(model);
            }

            return models;
        }
        public static List<TextValuePairModel> ToTextValuePairModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<TextValuePairModel>();
            while (readers.Read())
            {
                var model = new TextValuePairModel
                {
                    label = Convert.IsDBNull(readers["Name"]) ? string.Empty : Convert.ToString(readers["Name"]),
                    value = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"])
                };
                models.Add(model);
            }
            return models;
        }
        public static List<TextValuePairModelEmp> TextValuePairModelEmp(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<TextValuePairModelEmp>();
            while (readers.Read())
            {
                var model = new TextValuePairModelEmp
                {
                    
                    label = Convert.IsDBNull(readers["Name"]) ? string.Empty : Convert.ToString(readers["Name"]),
                    id = Convert.IsDBNull(readers["Id"]) ? string.Empty : Convert.ToString(readers["Id"]),

                };
                models.Add(model);
            }
            return models;
        }
    }
}
