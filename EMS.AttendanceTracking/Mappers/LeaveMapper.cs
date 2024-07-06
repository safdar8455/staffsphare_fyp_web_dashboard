using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Mappers
{
    public static class LeaveMapper
    {
        public static List<LeaveModel> ToEmployeeLeaveMapperModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<LeaveModel>();

            while (readers.Read())
            {
                var model = new LeaveModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    CompanyId = Convert.ToInt32(readers["CompanyId"]),
                    ApproverSerialId = Convert.IsDBNull(readers["ApproverSerialId"]) ? (int?)null : Convert.ToInt32(readers["ApproverSerialId"]),

                    UserId = Convert.ToString(readers["PortalUserId"]),
                    RequestNo = Convert.IsDBNull(readers["RequestNo"]) ? string.Empty : Convert.ToString(readers["RequestNo"]),

                    EmployeeId = Convert.ToInt32(readers["EmployeeId"]),
                    FromDate = Convert.ToDateTime(readers["FromDate"]),
                    ToDate = Convert.ToDateTime(readers["ToDate"]),
                    IsHalfDay = Convert.IsDBNull(readers["IsHalfDay"]) ? (bool?)null : Convert.ToBoolean(readers["IsHalfDay"]),
                    StatusId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                    LeaveTypeId = Convert.ToInt32(readers["LeaveTypeId"]),
                    LeaveReason = Convert.IsDBNull(readers["LeaveReason"]) ? string.Empty : Convert.ToString(readers["LeaveReason"]),
                    CreatedAt = Convert.IsDBNull(readers["CreatedAt"]) ? string.Empty : Convert.ToDateTime(readers["CreatedAt"]).ToString(),
                    IsApproved = Convert.IsDBNull(readers["IsApproved"]) ? false : Convert.ToBoolean(readers["IsApproved"]),
                    IsRejected = Convert.IsDBNull(readers["IsRejected"]) ? false : Convert.ToBoolean(readers["IsRejected"]),
                    IsCorrection = Convert.IsDBNull(readers["IsCorrection"]) ? false : Convert.ToBoolean(readers["IsCorrection"]),
                    ApprovedById = Convert.IsDBNull(readers["ApprovedById"]) ? string.Empty : Convert.ToString(readers["ApprovedById"]).ToString(),
                    ApprovedAt = Convert.IsDBNull(readers["ApprovedAt"]) ? (DateTime?)null : Convert.ToDateTime(readers["ApprovedAt"]),
                    EmployeeName = Convert.IsDBNull(readers["EmployeeName"]) ? string.Empty : Convert.ToString(readers["EmployeeName"]),
                    ApprovedBy = Convert.IsDBNull(readers["ApprovedBy"]) ? string.Empty : Convert.ToString(readers["ApprovedBy"]),
                    NextApproverId = Convert.IsDBNull(readers["NextApproverId"]) ? string.Empty : Convert.ToString(readers["NextApproverId"]),

                    LeaveType = Convert.IsDBNull(readers["LeaveType"]) ? string.Empty : Convert.ToString(readers["LeaveType"]),
                };
                models.Add(model);
            }

            return models;
        }
    }
}
