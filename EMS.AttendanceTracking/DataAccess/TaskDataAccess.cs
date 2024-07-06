using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.DataAccess
{
    public class TaskDataAccess : BaseDatabaseHandler, ITask
    {
        public ResponseModel Delete(string id)
        {
            string err = string.Empty;
            string sql = @"DELETE TaskAttachments WHERE TaskId=@id
                            DELETE Task WHERE Id = @id ";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id },

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            var response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }
        public ResponseModel AddOrUpdate(TaskModel model, List<TaskAttachment> attachmentsModel)
        {
            var errMessage = string.Empty;
            Database db = GetSQLDatabase();
            using (DbConnection conn = db.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    var returnOb = SaveTask(model, db, trans);
                    if (returnOb.Success && model.AssignedToldList != null)
                    {
                        IfTaskExist(returnOb.ReturnCode, db, trans);
                        foreach (var item in model.AssignedToldList )
                        {

                            TasKAssignTo taskModel = new TasKAssignTo()
                            {
                                TaskId = returnOb.ReturnCode,
                                AssignedToId = item.id,                                  
                            };

                            SaveTaskAssignTo(taskModel, db, trans);
                            
                        }
                    }
                    if (returnOb.Success && attachmentsModel != null)
                    {
                        foreach (var item in attachmentsModel)
                        {
                            if (!string.IsNullOrEmpty(item.BlobName))
                            {
                                TaskAttachment taModel = new TaskAttachment()
                                {
                                    Id = item.Id == null ? Guid.NewGuid().ToString() : item.Id,
                                    TaskId = returnOb.ReturnCode,
                                    BlobName = item.BlobName,
                                    FileName = item.FileName,
                                    UpdatedAt = DateTime.Now,
                                    UpdatedById = model.CreatedById
                                };

                                SaveTaskAttachment(taModel, db, trans);
                            }
                        }

                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    errMessage = ex.Message;
                }
            }
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage), Message = string.IsNullOrEmpty(errMessage) ? "Saved sucessfully" : "Problem in save" };
        }
        public ResponseModel SaveTask(TaskModel model, Database db, DbTransaction trans)
        {
            var errMessage = string.Empty;
            var guid = Guid.NewGuid().ToString();
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =string.IsNullOrEmpty(model.Id)?guid:model.Id},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Title", ParamValue =model.Title},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Description", ParamValue = model.Description},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedAt", ParamValue = DateTime.UtcNow,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedById", ParamValue = string.IsNullOrEmpty(model.CreatedById)?null:model.CreatedById},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@AssignedToId", ParamValue =string.IsNullOrEmpty(model.AssignedTold)?null:model.AssignedTold},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@StatusId", ParamValue = model.StatusId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskGroupId", ParamValue = model.TaskGroupId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@DueDate", ParamValue = model.DueDate.HasValue?model.DueDate:(DateTime?)null,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CompanyId", ParamValue = model.CompanyId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@PriorityId", ParamValue = model.PriorityId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskTypeId", ParamValue = model.TaskTypeId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@ContractNumber", ParamValue = model.ContractNumber},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@OutDoorAddr", ParamValue =model.OutDoorAddr},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@OurDoorLat", ParamValue =model.OurDoorLat},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@OurDoorLong", ParamValue =model.OurDoorLong},

                };

            const string sql = @"IF NOT EXISTS(SELECT TOP 1 P.Id FROM Task P WHERE P.Id=@Id)
                                BEGIN
                                 DECLARE @tNo INT=0
                                 SELECT @tNo=count(t.Id) FROM Task T WHERE T.CompanyId=@CompanyId
                                 INSERT INTO Task(Id,TaskNo,Title,Description,CreatedAt,CreatedById,AssignedToId,StatusId,DueDate,CompanyId,PriorityId,TaskTypeId,ContractNumber,OutDoorAddr,OurDoorLat,OurDoorLong)
				                 VALUES(@Id,@tNo+1,@Title,@Description,@CreatedAt,@CreatedById,@AssignedToId,@StatusId,@DueDate,@CompanyId,@PriorityId,@TaskTypeId,@ContractNumber,@OutDoorAddr,@OurDoorLat,@OurDoorLong)
                                END
                                ELSE
                                BEGIN
                                  UPDATE Task SET Title=@Title,Description=@Description,AssignedToId=@AssignedToId,PriorityId=@PriorityId,OutDoorAddr=@OutDoorAddr,OurDoorLat=@OurDoorLat,OurDoorLong=@OurDoorLong,
                                    StatusId=@StatusId,TaskGroupId=@TaskGroupId,DueDate=@DueDate,UpdatedById=@CreatedById,UpdatedAt=@CreatedAt,TaskTypeId=@TaskTypeId, ContractNumber=@ContractNumber WHERE Id=@Id
                                END";
            DBExecCommandExTran(sql, queryParamList, trans, db, ref errMessage);
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage), ReturnCode = string.IsNullOrEmpty(model.Id) ? guid : model.Id };

        }
        public ResponseModel IfTaskExist(string TaskId, Database db, DbTransaction trans)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskId", ParamValue = TaskId},
                   
                };

            const string sql = @"IF EXISTS(SELECT TOP 1 P.Id FROM TasKAssignTo P WHERE P.TaskId=@TaskId)
                                BEGIN
                               	delete from [dbo].[TasKAssignTo] where TaskId=@TaskId
                                END";
            DBExecCommandExTran(sql, queryParamList, trans, db, ref errMessage);
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }
        public ResponseModel SaveTaskAssignTo(TasKAssignTo model, Database db, DbTransaction trans)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue = string.IsNullOrEmpty(model.Id)?null:model.Id},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskId", ParamValue =model.TaskId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@AssignedToId", ParamValue =model.AssignedToId},
                };

            const string sql = @"INSERT INTO TasKAssignTo(TaskId,AssignedToId) 
                                VALUES (@TaskId,@AssignedToId)";
            DBExecCommandExTran(sql, queryParamList, trans, db, ref errMessage);
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }

        public ResponseModel SaveTaskAttachment(TaskAttachment model, Database db, DbTransaction trans)
        {
            var errMessage = string.Empty;
            var guid = Guid.NewGuid().ToString();
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue = string.IsNullOrEmpty(model.Id)?guid:model.Id},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskId", ParamValue =model.TaskId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@FileName", ParamValue =model.FileName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@BlobName", ParamValue =model.BlobName},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UpdatedAt", ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@UpdatedById", ParamValue =model.UpdatedById},
                };

            const string sql = @"IF NOT EXISTS(SELECT TOP 1 P.Id FROM TaskAttachments P WHERE P.Id=@Id)
                                BEGIN
                               	INSERT INTO TaskAttachments(Id,TaskId,FileName,BlobName,UpdatedAt,UpdatedById) 
                                VALUES (@Id,@TaskId,@FileName,@BlobName,@UpdatedAt,@UpdatedById)
                                END
                                ELSE 
                                BEGIN
                                UPDATE TaskAttachments SET FileName=@FileName,
                                BlobName=@BlobName,UpdatedById=@UpdatedById,UpdatedAt=@UpdatedAt
	                            WHERE Id=@Id
                                END";
            DBExecCommandExTran(sql, queryParamList, trans, db, ref errMessage);
            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }
        public List<TaskAttachment> GetTaskAttachments(string taskId)
        {
            const string sql = @"SELECT * FROM TaskAttachments where TaskId=@taskId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@taskId", ParamValue = taskId}
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.ToTaskAttachment);
        }
        public List<TaskModel> GetTaskList(TaskModel sModel)
        {

            const string sql = @"SELECT DISTINCT T.*,C.FullName AssignToName,CreatedBy.FullName CreatedByName,UpdatedBy.FullName UpdatedByName 
                                FROM Task t
								LEFT JOIN TasKAssignTo ta on t.Id=ta.TaskId
                                LEFT JOIN UserCredentials C ON ta.AssignedToId=C.Id 
                                    LEFT JOIN UserCredentials CreatedBy ON T.CreatedById=CreatedBy.Id 
                                    LEFT JOIN UserCredentials UpdatedBy ON T.UpdatedById=UpdatedBy.Id 
                                    where 
                                   (@CreatedById is null or t.CreatedById=@CreatedById)
                                    and (@AssignedToId is null or ta.AssignedToId=@AssignedToId)
                                    and (@TaskGroupId is null or t.TaskGroupId=@TaskGroupId)";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedById", ParamValue =string.IsNullOrEmpty(sModel.CreatedById)?null:sModel.CreatedById},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@AssignedToId", ParamValue = string.IsNullOrEmpty(sModel.AssignedTold)?null:sModel.AssignedTold},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskGroupId", ParamValue = sModel.TaskGroupId},
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.ToTask);
        }

        public List<TaskModel> GetAssignedToMeTasks(TaskModel sModel)
        {

            const string sql = @"SELECT T.*,C.FullName AssignToName,CreatedBy.FullName CreatedByName,UpdatedBy.FullName UpdatedByName 
                                FROM Task t
                                LEFT JOIN TasKAssignTo ta on t.Id=ta.TaskId
                                LEFT JOIN UserCredentials C ON Ta.AssignedToId=C.Id 
                                    LEFT JOIN UserCredentials CreatedBy ON T.CreatedById=CreatedBy.Id 
                                    LEFT JOIN UserCredentials UpdatedBy ON T.UpdatedById=UpdatedBy.Id 
                                    where 
                                   (@CreatedById is null or t.CreatedById=@CreatedById)
                                    and (@AssignedToId is null or t.AssignedToId=@AssignedToId)
                                    and (@TaskGroupId is null or t.TaskGroupId=@TaskGroupId)";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@CreatedById", ParamValue =string.IsNullOrEmpty(sModel.CreatedById)?null:sModel.CreatedById},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@AssignedToId", ParamValue = string.IsNullOrEmpty(sModel.AssignedTold)?null:sModel.AssignedTold},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskGroupId", ParamValue = sModel.TaskGroupId},
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.ToTask);
        }
        
        public List<TaskModel> GetRelatedToMeTaskList(TaskModel sModel)
        {
            const string sql = @"SELECT T.*,C.FullName AssignToName,CreatedBy.FullName CreatedByName,UpdatedBy.FullName UpdatedByName 
                                FROM Task t
                                LEFT JOIN TasKAssignTo ta on t.Id=ta.TaskId
                                LEFT JOIN UserCredentials C ON Ta.AssignedToId=C.Id  
                                    LEFT JOIN UserCredentials CreatedBy ON T.CreatedById=CreatedBy.Id 
                                    LEFT JOIN UserCredentials UpdatedBy ON T.UpdatedById=UpdatedBy.Id  where (T.AssignedToId=@userId OR T.CreatedById=@userId)";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@userId", ParamValue =sModel.AssignedTold}
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.ToTask);
        }
        public List<TaskModel> GetTasksByCompanyId(string companyId)
        {
            const string sql = @"SELECT T.*,C.FullName AssignToName,CreatedBy.FullName CreatedByName,UpdatedBy.FullName UpdatedByName 
                                FROM Task t
                                LEFT JOIN UserCredentials C ON T.AssignedToId=C.Id  
                                    LEFT JOIN UserCredentials CreatedBy ON T.CreatedById=CreatedBy.Id 
                                    LEFT JOIN UserCredentials UpdatedBy ON T.UpdatedById=UpdatedBy.Id where t.CompanyId=@companyId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue =companyId}
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.ToTask);
        }
        public ResponseModel DeleteTask(string id)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =id},
                };
            const string sql2 = @"IF EXISTS(SELECT TOP 1 P.Id FROM TaskAttachments P WHERE P.TaskId=@Id)
                                BEGIN
                                    DELETE FROM TaskAttachments WHERE TaskId=@Id
                                END";
            DBExecCommandEx(sql2, queryParamList, ref errMessage);
            const string sql = @"DELETE FROM Task WHERE Id=@Id";
            DBExecCommandEx(sql, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }
        public ResponseModel DeleteAttachment(string id)
        {
            var errMessage = string.Empty;
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@Id", ParamValue =id},
                };
            const string sql2 = @"IF EXISTS(SELECT TOP 1 P.Id FROM TaskAttachments P WHERE P.Id=@Id)
                                BEGIN
                                    DELETE FROM TaskAttachments WHERE Id=@Id
                                END";
            DBExecCommandEx(sql2, queryParamList, ref errMessage);

            return new ResponseModel { Success = string.IsNullOrEmpty(errMessage) };
        }
        List<TaskModel> ITask.GetAll(int? cId)
        {
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@cId", ParamValue =cId},
                };
            const string sql = @"SELECT T.*,u.LoginID AssignToName
                                FROM Task T
                                left join UserCredentials u on u.Id=t.AssignedToId where t.CompanyId=@cId order by t.TaskNo";
            return ExecuteDBQuery(sql, queryParamList, TaskMappers.ToModel);
        }
        List<TaskModel> ITask.GetAllDateFilter(int? cId, DateTime StartDate, DateTime EndDate)
        {
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@cId", ParamValue =cId},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@startDate", ParamValue = StartDate,DBType=DbType.DateTime},
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@endDate", ParamValue = EndDate,DBType=DbType.DateTime}
                };
            const string sql = @"SELECT T.*,u.LoginID AssignToName
                                FROM Task T
                                left join UserCredentials u on u.Id=t.AssignedToId where t.CompanyId=@cId and (CONVERT(DATE,T.CreatedAt) BETWEEN convert(date,@endDate) AND convert(date,@startDate))";
            return ExecuteDBQuery(sql, queryParamList, TaskMappers.ToModel);
        }
        public List<TextValuePairModel> GetEmployeeAsTextValue(int companyId)
        {
            const string sql = @"SELECT U.Id,U.LoginID Name FROM UserCredentials U where U.CompanyId=@companyId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId}
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.ToTextValuePairModel);
        }
        public List<TextValuePairModelEmp> GetEmployeeDDLAsTextValue(int companyId)
        {
            const string sql = @"SELECT U.Id,U.LoginID Name FROM UserCredentials U where U.CompanyId=@companyId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@companyId", ParamValue = companyId}
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.TextValuePairModelEmp);
        }
        public List<TextValuePairModelEmp> GetEmployeeAssignedTextValue(string TaskId)
        {
            const string sql = @"select t.AssignedToId as Id,u.FullName as Name from [TasKAssignTo] t
                                join UserCredentials u on u.Id=t.AssignedToId
                                where TaskId=@TaskId";
            var queryParamList = new QueryParamList
                {
                    new QueryParamObj { ParamDirection = ParameterDirection.Input, ParamName = "@TaskId", ParamValue = TaskId}
                };
            return ExecuteDBQuery(sql, queryParamList, TaskMapper.TextValuePairModelEmp);
        }

        public static class TaskMappers
        {
            public static List<TaskModel> ToModel(DbDataReader readers)
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
                        DueDate = Convert.IsDBNull(readers["DueDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["DueDate"]),
                        CreatedById = Convert.IsDBNull(readers["CreatedById"]) ? string.Empty : Convert.ToString(readers["CreatedById"]),
                        CreatedAt = Convert.IsDBNull(readers["CreatedAt"]) ? (DateTime?)null : Convert.ToDateTime(readers["CreatedAt"]),
                        StatusId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                        TaskGroupId = Convert.IsDBNull(readers["TaskGroupId"]) ? (int?)null : Convert.ToInt32(readers["TaskGroupId"]),
                        AssignedTold = Convert.IsDBNull(readers["AssignedToId"]) ? string.Empty : Convert.ToString(readers["AssignedToId"]),
                        AssignToName = Convert.IsDBNull(readers["AssignToName"]) ? string.Empty : Convert.ToString(readers["AssignToName"]),
                        CompanyId = Convert.IsDBNull(readers["CompanyId"]) ? (int?)null : Convert.ToInt32(readers["CompanyId"]),
                        TaskNo = Convert.IsDBNull(readers["TaskNo"]) ? (int?)null : Convert.ToInt32(readers["TaskNo"]),
                        PriorityId = Convert.IsDBNull(readers["PriorityId"]) ? (int?)null : Convert.ToInt32(readers["PriorityId"]),
                        UpdatedById = Convert.IsDBNull(readers["UpdatedById"]) ? string.Empty : Convert.ToString(readers["UpdatedById"]),
                        UpdatedAt = Convert.IsDBNull(readers["UpdatedAt"]) ? (DateTime?)null : Convert.ToDateTime(readers["UpdatedAt"]),
                        DueDateStr = Convert.IsDBNull(readers["DueDate"]) ? string.Empty : Convert.ToDateTime(readers["DueDate"]).ToString(Constants.DateFormat),
                        TaskTypeId = Convert.IsDBNull(readers["TaskTypeId"])?(int?)null:Convert.ToInt32(readers["TaskTypeId"]),
                        ContractNumber = Convert.IsDBNull(readers["ContractNumber"]) ? string.Empty : Convert.ToString(readers["ContractNumber"]),
                        OutDoorAddr = Convert.IsDBNull(readers["OutDoorAddr"]) ? string.Empty : Convert.ToString(readers["OutDoorAddr"]),
                        OurDoorLat = Convert.IsDBNull(readers["OurDoorLat"]) ? (decimal?)null : Convert.ToDecimal(readers["OurDoorLat"]),
                        OurDoorLong = Convert.IsDBNull(readers["OurDoorLong"]) ? (decimal?)null : Convert.ToDecimal(readers["OurDoorLong"])

                    };
                    models.Add(model);

                }
                return models;
            }
        }
    }
  
}
