using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Models;
using Ems.BusinessTracker.DataAccess.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.DataAccess
{
    public class EmployeeDataAccess : BaseDatabaseHandler, IEmployee
    {
        public ResponseModel SaveEmployee(EmployeeDetailsModel model)
        {
            var returnId = (int?)null;
            var db = GetSQLDatabase();
            var message = string.Empty;
            using (DbConnection conn = db.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    returnId = SaveEmployeeeBasicInfo(model, db, trans);
                    if (returnId > 0)
                    {
                        SaveEmployeeSalaryStructure(model, db, trans, returnId.Value);
                        SaveEmployeeOtherDetails(model, db, trans, returnId.Value);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    message = ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            var success = string.IsNullOrEmpty(message);
            return new ResponseModel { Success = success, Message = success ? string.Empty : model.EmployeeCode };
        }
        private int SaveEmployeeeBasicInfo(EmployeeDetailsModel employee, Database db, DbTransaction trans)
        {
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_Save_BasicInfo");
            var employeePassword = string.Empty;
            if(!string.IsNullOrEmpty(employee.Password))
              employeePassword = CryptographyHelper.CreateMD5Hash(employee.Password);

            var queryParamList = new QueryParamList
            {
                new QueryParamObj { ParamName = "@Id",  ParamValue =employee.Id},
                new QueryParamObj { ParamName = "@EmployeeCode",  ParamValue =employee.EmployeeCode},
                new QueryParamObj { ParamName = "@EmployeeName",  ParamValue =employee.EmployeeName},
                new QueryParamObj { ParamName = "@LoginID",  ParamValue =employee.LoginID},
                new QueryParamObj { ParamName = "@EmployeePassword",  ParamValue =employeePassword},
                new QueryParamObj { ParamName = "@PortalUserId",  ParamValue =Guid.NewGuid().ToString()},
                new QueryParamObj { ParamName = "@EmployeeStatusId",  ParamValue =employee.EmployeeStatusId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@HasAccessQrCodeScan",  ParamValue =employee.HasAccessQrCodeScan,DBType=DbType.Boolean},
                new QueryParamObj { ParamName = "@DesignationId",  ParamValue =employee.DesignationId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@DepartmentId",  ParamValue =employee.DepartmentId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@DateOfJoining",  ParamValue =employee.JoiningDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@NationalityId",  ParamValue =employee.NationalityId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@DateOfBirth",  ParamValue =employee.DateOfBirth,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@GenderId",  ParamValue =employee.GenderId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@PassportNo",  ParamValue =employee.PassportNo},
                new QueryParamObj { ParamName = "@PassportIssueDate",  ParamValue =employee.PassportIssueDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@PassportExpiryDate",  ParamValue =employee.PassportExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@QID",  ParamValue =employee.QID},
                new QueryParamObj { ParamName = "@QIDExpiryDate",  ParamValue =employee.QIDExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@WorkingCompanyId",  ParamValue =employee.WorkingCompanyId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@SponsorshipId",  ParamValue =employee.SponsorshipId, DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@VisaNo",  ParamValue =employee.VisaNo},
                new QueryParamObj { ParamName = "@VisaExpirayDate",  ParamValue =employee.VisaExpirayDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@WorkLocationId",  ParamValue =employee.WorkLocationId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@CompanyAccomodation", ParamValue =employee.CompanyAccomodation},
                new QueryParamObj { ParamName = "@ProjectId",  ParamValue =employee.ProjectId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@HealthCardNo",  ParamValue =employee.HealthCardNo},
                new QueryParamObj { ParamName = "@HealthCardExpiryDate",  ParamValue =employee.HealthCardExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@MobileNo",  ParamValue =employee.MobileNo},
                new QueryParamObj { ParamName = "@Insurance",  ParamValue =employee.Insurance},
                new QueryParamObj { ParamName = "@InsuranceExpirayDate",  ParamValue =employee.InsuranceExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@ActionAt",  ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@ActionById",  ParamValue = employee.ActionById},
                new QueryParamObj { ParamName = "@ImageFileName",  ParamValue =employee.ImageFileName},
                new QueryParamObj { ParamName = "@FoodHandling",  ParamValue =employee.FoodHandling},
                new QueryParamObj { ParamName = "@FoodhandlingIssueDate",  ParamValue =employee.FoodHandlingIssueDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@FoodhandlingExpiryDate", ParamValue =employee.FoodhandlingExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@LastWorkingDate",  ParamValue =employee.LastWorkngDate,DBType=DbType.DateTime}
            };
            var identityParam = new QueryParamObj
            {
                ParamName = "@ReturnId",
                ParamDirection = ParameterDirection.Output,
                DBType = DbType.Int32
            };
            queryParamList.Add(identityParam);

            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
            identityParam.ParamValue = db.GetParameterValue(templateCommand, identityParam.ParamName);
            return Convert.ToInt32(identityParam.ParamValue);
        }
        private void SaveEmployeeSalaryStructure(EmployeeDetailsModel model, Database db, DbTransaction trans, long pk)
        {
            if (model == null)
                return;
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_SaveSalaryStructureBatch");
            var queryParamList = new QueryParamList
            {
                new QueryParamObj { ParamName = "@EmployeeId",  ParamValue =pk,DBType=DbType.Int64},
                new QueryParamObj { ParamName = "@BasicPay",  ParamValue =model.BasicPay,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@Housing",  ParamValue =model.Housing,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@Transport",  ParamValue =model.Transport,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@Telephone",  ParamValue =model.Telephone,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@FoodAllowance",  ParamValue =model.FoodAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@OtherAllowancce",  ParamValue =model.OtherAllowances,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@TeamLeadAllowance",  ParamValue =model.TeamLeaderAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@CityCompensatoryAllowance",  ParamValue =model.CityCompensatoryAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@PersonalAllowance",  ParamValue =model.PersonalAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@OutsideAllowance",  ParamValue =model.OutSideAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@NetSalary",  ParamValue =model.NetSalary,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@BankName",  ParamValue =model.BankName},
                new QueryParamObj { ParamName = "@EmployeeAccount",  ParamValue =model.EmployeeAccount},
                new QueryParamObj { ParamName = "@SalaryCategory",  ParamValue =model.SalaryCategory},
            };
            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
        }

        private void SaveEmployeeOtherDetails(EmployeeDetailsModel model, Database db, DbTransaction trans, long pk)
        {
            if (model == null)
                return;
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_Save_OtherDetails");
            var queryParamList = new QueryParamList
            {
                new QueryParamObj { ParamName = "@EmployeeId",  ParamValue =pk,DBType=DbType.Int64},                
                new QueryParamObj { ParamName = "@ContractPeriodYear",  ParamValue =model.ContractPeriod,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@LaborContract",  ParamValue =model.LaborContract,DBType=DbType.String},
                new QueryParamObj { ParamName = "@ContractIssueDate",  ParamValue =model.ContractIssueDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@ContractExpiryDate",  ParamValue =model.ContractExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@CompanyID",  ParamValue =model.CompanyID,DBType=DbType.String},
                new QueryParamObj { ParamName = "@MotherTongueId",  ParamValue =model.MotherTongueId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@MaritalStatusId",  ParamValue =model.MaritalStatusId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@ChildrenNo",  ParamValue =model.ChildrenNo,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@ReligionId",  ParamValue =model.ReligionId, DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@PreviousCompany",  ParamValue =model.PreviousCompany},
                new QueryParamObj { ParamName = "@CountryId",  ParamValue =model.CountryId , DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@HomeAirportId",  ParamValue =model.HomeAirportId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@CompanyEmailID",  ParamValue =model.CompanyEmailID},
                new QueryParamObj { ParamName = "@EmailIDs",  ParamValue =model.EmailIDs},
                new QueryParamObj { ParamName = "@EmployeeGroupId",  ParamValue =model.EmployeeGroupId,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@EmployeeSubGroup",  ParamValue =model.EmployeeSubGroup,DBType=DbType.String},
                new QueryParamObj { ParamName = "@EmployeeOfTheMonth",  ParamValue =model.EmployeeOfTheMonth},
                new QueryParamObj { ParamName = "@UpdaLicense",  ParamValue =model.EpdaLicense},
                new QueryParamObj { ParamName = "@Registration",  ParamValue =model.Registration},
                new QueryParamObj { ParamName = "@Grade",  ParamValue =model.Grade,DBType=DbType.String},
                new QueryParamObj { ParamName = "@UdpaExpiryDate",  ParamValue =model.UpdaExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@Remarks",  ParamValue =model.Remarks},
                new QueryParamObj { ParamName = "@LeaveEntitlement",  ParamValue =model.LeaveEntitlement,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@AirTicketEntitlementTotalMonth",  ParamValue =model.AirTicketEntitlement,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@HiredThrough",  ParamValue =model.HiredThrough,DBType=DbType.String},
                new QueryParamObj { ParamName = "@LeavePolicyCode",  ParamValue =model.LeavePolicyCode}
            };
            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
        }
        public List<CompanyModel> GetCompanyList()
        {
            string err = string.Empty;
            string sql = @"select * from Company";
            var results = ExecuteDBQuery(sql, null, EmployeeMapper.ToCompanyModel);
            return results;
        }
        public ResponseModel Delete(int id)
        {
            string err = string.Empty;
            string sql = @"DECLARE @eId NVARCHAR(50)
                        SELECT TOP 1 @eId=E.LoginID FROM  Employee E WHERE E.Id=@id
                        IF EXISTS(SELECT TOP 1 * FROM UserCredentials U WHERE U.LoginID=@eId)
                        BEGIN
                            DELETE FROM UserCredentials WHERE LoginID=@eId
                        END
                        DELETE FROM EmployeeOtherDetails WHERE EmployeeId=@id
                        DELETE FROM EmployeeSalaryStructure WHERE EmployeeId=@id
                        DELETE FROM LeaveApplication WHERE EmployeeId=@id
                        DELETE FROM Employee WHERE Id = @id";

            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@id", ParamValue =id ,DBType = DbType.Int32},

                };

            DBExecCommandEx(sql, queryParamList, ref err);
            var response = new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Delete sucessfully" : "Already used" };

            return response;
        }
        public ResponseModel UpdateEmployeeImage(EmployeeDetailsModel model)
        {
            string err = string.Empty;
            var sql = @"UPDATE Employee SET
	                        ImageFileName=@UploadedFileName
	                        WHERE Id=@Id ";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@Id", ParamValue =model.Id ,DBType = DbType.Int32},
                    new QueryParamObj { ParamName = "@UploadedFileName", ParamValue =model.AttachedDocument.UploadedFileName}
                };
            DBExecCommandEx(sql, queryParamList, ref err);
            return new ResponseModel { Success = string.IsNullOrEmpty(err), Message = string.IsNullOrEmpty(err) ? "Updated sucessfully" : "Problem in Update" };
        }
        public List<EmployeeExportModel> GetAllEmp()
        {
            const string sql = @"SELECT E.Id,e.PortalUserId,e.EmployeeName, D.DepartmentName,DG.Name DesignationName,e.WorkingCompanyId
                                FROM Employee E
                                LEFT JOIN Department D ON E.DepartmentId=D.Id
                                LEFT JOIN Designation DG ON E.DesignationId=DG.Id";
            return ExecuteDBQuery(sql, null, EmployeeMapper.ToEmpListModel);
        }

        public List<EmployeeExportModel> GetAll()
        {
            const string sql = @"SELECT DISTINCT E.Id,e.EmployeeCode,e.EmployeeName,e.IsDeleteable,e.LoginID,e.MobileNo,e.SponsorshipId,c.CompanyAdminEmail,
                                D.DepartmentName,DG.Name DesignationName,lm.EmployeeName LineManagerName,E.ImageFileName
                                ,c.HrDirectorCode,HrDirector.EmployeeName HrDirectorName,DManager.EmployeeName DepartmentManagerName
                                ,Project.Name ProjectName,Nationality.Name NationalityName,e.DateOfBirth
                                ,Gender.Name GenderName,e.PassportNo,e.PassportIssueDate,e.PassportExpiryDate
                                ,e.QID,e.QIDExpiryDate,C.CompanyName WorkingCompanyName,e.Sponsorship,e.VisaNo
                                ,e.VisaExpirayDate,WorkLocation.Name WorkLocationName,E.CompanyAccomodation,E.HealthCardNo
                                ,E.HealthCardExpiryDate,E.Insurance,E.InsuranceExpirayDate,E.FoodHandling,E.FoodhandlingIssueDate
                                ,E.FoodhandlingExpiryDate,E.LastWorkingDate,E.DateOfJoining,ST.BasicPay,ST.Housing,ST.Transport
                                ,ST.Telephone,ST.FoodAllowance,ST.OtherAllowancce,ST.TeamLeadAllowance,ST.CityCompensatoryAllowance
                                ,ST.PersonalAllowance,ST.OutsideAllowance,ST.NetSalary,ST.BankName,ST.EmployeeAccount,ST.SalaryCategory
                                ,lp.PolicyCode LeavePolicyCode,lp.Description LeavePolicy,OD.LeaveEntitlement,OD.AirTicketEntitlementTotalMonth
                                ,OD.HiredThrough,OD.ContractPeriodYear,OD.LaborContract,OD.ContractIssueDate,OD.ContractExpiryDate
                                ,OD.CompanyID,OD.MotherTongue,MaritalStatus.Name MaritalStatusName,od.ChildrenNo,Religion.Name ReligionName
                                ,od.PreviousCompany,Country.Name CountryName,od.HomeAirport,od.CompanyEmailID,od.EmailIDs,od.EmployeeGroup,
                                od.EmployeeSubGroup,od.EmployeeOfTheMonth,od.UpdaLicense,od.Registration,od.Grade,od.UdpaExpiryDate,od.Remarks
                                ,od.LeavePolicyId,od.MaritalStatusId,od.ReligionId,od.CountryId,e.DesignationId,e.DepartmentId,e.StatusId
                                ,e.ProjectId,e.NationalityId,e.GenderId,e.WorkingCompanyId,e.WorkLocationId
                                FROM Employee E
                                LEFT JOIN Department D ON E.DepartmentId=D.Id
                                LEFT JOIN Designation DG ON E.DesignationId=DG.Id
                                LEFT JOIN Employee LM ON d.LineManagerCode=lm.EmployeeCode
                                LEFT JOIN EmployeeSalaryStructure ST ON E.Id=ST.EmployeeId
                                LEFT JOIN EmployeeOtherDetails OD ON E.Id=OD.EmployeeId
                                LEFT JOIN InputHelp Project ON E.ProjectId=Project.Id
                                LEFT JOIN InputHelp Nationality ON E.NationalityId=Nationality.Id
                                LEFT JOIN InputHelp Gender ON E.GenderId=Gender.Id
                                LEFT JOIN Company C ON E.WorkingCompanyId=C.Id
                                LEFT JOIN InputHelp WorkLocation ON E.WorkLocationId=WorkLocation.Id
                                LEFT JOIN LeavePolicy LP ON OD.LeavePolicyId=LP.Id
                                LEFT JOIN InputHelp MaritalStatus ON od.MaritalStatusId=MaritalStatus.Id
                                LEFT JOIN InputHelp Religion ON od.ReligionId=Religion.Id
                                LEFT JOIN InputHelp Country ON od.CountryId=Country.Id
                                LEFT JOIN Employee HrDirector ON c.HrDirectorCode=HrDirector.EmployeeCode
                                LEFT JOIN Employee DManager ON D.DepartmentManagerCode=DManager.EmployeeCode";
            return ExecuteDBQuery(sql, null, EmployeeMapper.ToListModel);
        }
        public List<EmployeeDetailsModel> GetEmployeeDetails(long? employeeId)
        {
            const string sql = @"SELECT E.Id,e.EmployeeCode,e.EmployeeName,e.LoginID,e.MobileNo,e.SponsorshipId,e.HasAccessQrCodeScan,
                                D.DepartmentName,DG.Name DesignationName,lm.EmployeeName LineManagerName,E.ImageFileName
                                ,c.HrDirectorCode,HrDirector.EmployeeName HrDirectorName,DManager.EmployeeName DepartmentManagerName
                                ,Project.Name ProjectName,Nationality.Name NationalityName,e.DateOfBirth
                                ,Gender.Name GenderName,e.PassportNo,e.PassportIssueDate,e.PassportExpiryDate
                                ,e.QID,e.QIDExpiryDate,C.CompanyName WorkingCompanyName,e.VisaNo
                                ,e.VisaExpirayDate,WorkLocation.Name WorkLocationName,E.CompanyAccomodation,E.HealthCardNo
                                ,E.HealthCardExpiryDate,E.Insurance,E.InsuranceExpirayDate,E.FoodHandling,E.FoodhandlingIssueDate
                                ,E.FoodhandlingExpiryDate,E.LastWorkingDate,E.DateOfJoining,ST.BasicPay,ST.Housing,ST.Transport
                                ,ST.Telephone,ST.FoodAllowance,ST.OtherAllowancce,ST.TeamLeadAllowance,ST.CityCompensatoryAllowance
                                ,ST.PersonalAllowance,ST.OutsideAllowance,ST.NetSalary,ST.BankName,ST.EmployeeAccount,ST.SalaryCategory
                                ,lp.PolicyCode LeavePolicyCode,lp.Description LeavePolicy,OD.LeaveEntitlement,OD.AirTicketEntitlementTotalMonth
                                ,OD.HiredThrough,OD.ContractPeriodYear,OD.LaborContract,OD.ContractIssueDate,OD.ContractExpiryDate
                                ,OD.CompanyID,OD.MotherTongueId,MaritalStatus.Name MaritalStatusName,od.ChildrenNo,Religion.Name ReligionName
                                ,od.PreviousCompany,Country.Name CountryName,od.HomeAirportId,od.CompanyEmailID,od.EmailIDs,od.EmployeeGroupId,
                                od.EmployeeSubGroup,od.EmployeeOfTheMonth,od.UpdaLicense,od.Registration,od.Grade,od.UdpaExpiryDate,od.Remarks
                                ,od.LeavePolicyId,od.MaritalStatusId,od.ReligionId,od.CountryId,e.DesignationId,e.DepartmentId,e.StatusId
                                ,e.ProjectId,e.NationalityId,e.GenderId,e.WorkingCompanyId,e.WorkLocationId
                                FROM Employee E
                                LEFT JOIN Department D ON E.DepartmentId=D.Id AND E.WorkingCompanyId=D.CompanyId
                                LEFT JOIN Designation DG ON E.DesignationId=DG.Id
                                LEFT JOIN Employee LM ON d.LineManagerCode=lm.EmployeeCode
                                LEFT JOIN EmployeeSalaryStructure ST ON E.Id=ST.EmployeeId
                                LEFT JOIN EmployeeOtherDetails OD ON E.Id=OD.EmployeeId
                                LEFT JOIN InputHelp Project ON E.ProjectId=Project.Id
                                LEFT JOIN InputHelp Nationality ON E.NationalityId=Nationality.Id
                                LEFT JOIN InputHelp Gender ON E.GenderId=Gender.Id
                                LEFT JOIN Company C ON E.WorkingCompanyId=C.Id
                                LEFT JOIN InputHelp WorkLocation ON E.WorkLocationId=WorkLocation.Id
                                LEFT JOIN LeavePolicy LP ON OD.LeavePolicyId=LP.Id
                                LEFT JOIN InputHelp MaritalStatus ON od.MaritalStatusId=MaritalStatus.Id
                                LEFT JOIN InputHelp Religion ON od.ReligionId=Religion.Id
                                LEFT JOIN InputHelp Country ON od.CountryId=Country.Id
                                LEFT JOIN Employee HrDirector ON c.HrDirectorCode=HrDirector.EmployeeCode
                                LEFT JOIN Employee DManager ON D.DepartmentManagerCode=DManager.EmployeeCode
                            WHERE (@employeeId is null or e.Id=@employeeId)";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@employeeId", ParamValue =employeeId ,DBType = DbType.Int64},

                };
            return ExecuteDBQuery(sql, queryParamList, EmployeeMapper.ToDetailsModel);
        }

        public ResponseModel UpdateEmployeeBatchFile(EmployeeBatchUploadModel model)
        {
            var returnId = (int?)null;
            var db = GetSQLDatabase();
            var message = string.Empty;
            using (DbConnection conn = db.CreateConnection())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                try
                {
                    returnId = SaveBatchFileEmployeeeBasicInfo(model, db, trans);
                    if (returnId > 0)
                    {
                        SaveBatchFileSalaryStructure(model, db, trans, returnId.Value);
                        SaveBatchFileOtherDetails(model, db, trans, returnId.Value);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    message = ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }
            var success = string.IsNullOrEmpty(message);
            return new ResponseModel { Success = success, Message = success ? string.Empty : model.EmployeeCode };
        }

        private int SaveBatchFileEmployeeeBasicInfo(EmployeeBatchUploadModel employee, Database db, DbTransaction trans)
        {
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_SaveBatch");
            var employeePassword = CryptographyHelper.CreateMD5Hash(employee.EmployeeCode);
            var queryParamList = new QueryParamList
            {

                new QueryParamObj { ParamName = "@EmployeeCode",  ParamValue =employee.EmployeeCode},
                new QueryParamObj { ParamName = "@EmployeePassword",  ParamValue =employeePassword},
                new QueryParamObj { ParamName = "@PortalUserCode",  ParamValue =employee.EmployeeCode},
                new QueryParamObj { ParamName = "@PortalUserId",  ParamValue =Guid.NewGuid().ToString()},
                new QueryParamObj { ParamName = "@Status",  ParamValue =employee.Status},
                new QueryParamObj { ParamName = "@Project",  ParamValue =employee.Project},
                new QueryParamObj { ParamName = "@EmployeeName",  ParamValue =employee.EmployeeName},
                new QueryParamObj { ParamName = "@Department",  ParamValue =employee.Department},
                new QueryParamObj { ParamName = "@Designation",  ParamValue =employee.Designation},
                new QueryParamObj { ParamName = "@MobileNo",  ParamValue =employee.MobileNo},

                new QueryParamObj { ParamName = "@Nationality",  ParamValue =employee.Nationality},
                new QueryParamObj { ParamName = "@DateOfBirth",  ParamValue =employee.DateOfBirth,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@Gender",  ParamValue =employee.Gender},
                new QueryParamObj { ParamName = "@PassportNo",  ParamValue =employee.PassportNo},
                new QueryParamObj { ParamName = "@PassportIssueDate",  ParamValue =employee.PassportIssueDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@PassportExpiryDate",  ParamValue =employee.PassportExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@QID",  ParamValue =employee.QID},
                new QueryParamObj { ParamName = "@QIDExpiryDate",  ParamValue =employee.QIDExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@WorkingCompany",  ParamValue =employee.WorkingCompany},

                new QueryParamObj { ParamName = "@Sponsorship",  ParamValue =employee.Sponsorship},
                new QueryParamObj { ParamName = "@VisaNo",  ParamValue =employee.VisaNo},
                new QueryParamObj { ParamName = "@VisaExpirayDate",  ParamValue =employee.VisaExpirayDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@WorkLocation",  ParamValue =employee.WorkLocation},
                new QueryParamObj { ParamName = "@CompanyAccomodation", ParamValue =employee.CompanyAccomodation},
                new QueryParamObj { ParamName = "@HealthCardNo",  ParamValue =employee.HealthCardNo},
                new QueryParamObj { ParamName = "@HealthCardExpiryDate",  ParamValue =employee.HealthCardExpiryDate,DBType=DbType.DateTime},

                new QueryParamObj { ParamName = "@Insurance",  ParamValue =employee.Insurance},
                new QueryParamObj { ParamName = "@InsuranceExpirayDate",  ParamValue =employee.InsuranceExpirayDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@FoodHandling",  ParamValue =employee.FoodHandling},
                new QueryParamObj { ParamName = "@FoodhandlingIssueDate",  ParamValue =employee.FoodhandlingIssueDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@FoodhandlingExpiryDate", ParamValue =employee.FoodhandlingExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@LastWorkingDate",  ParamValue =employee.LastWorkingDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@DateOfJoining",  ParamValue =employee.DateOfJoining,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@ActionAt",  ParamValue =DateTime.UtcNow,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@ActionById",  ParamValue = employee.ActionById},
            };
            var identityParam = new QueryParamObj
            {
                ParamName = "@ReturnId",
                ParamDirection = ParameterDirection.Output,
                DBType = DbType.Int32
            };
            queryParamList.Add(identityParam);

            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
            identityParam.ParamValue = db.GetParameterValue(templateCommand, identityParam.ParamName);
            return Convert.ToInt32(identityParam.ParamValue);
        }
        private void SaveBatchFileSalaryStructure(EmployeeBatchUploadModel model, Database db, DbTransaction trans, int pk)
        {
            if (model == null)
                return;
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_SaveSalaryStructureBatch");
            var queryParamList = new QueryParamList
            {
                new QueryParamObj { ParamName = "@EmployeeId",  ParamValue =pk,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@BasicPay",  ParamValue =model.BasicPay,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@Housing",  ParamValue =model.Housing,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@Transport",  ParamValue =model.Transport,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@Telephone",  ParamValue =model.Telephone,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@FoodAllowance",  ParamValue =model.FoodAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@OtherAllowancce",  ParamValue =model.OtherAllowancce,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@TeamLeadAllowance",  ParamValue =model.TeamLeadAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@CityCompensatoryAllowance",  ParamValue =model.CityCompensatoryAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@PersonalAllowance",  ParamValue =model.PersonalAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@OutsideAllowance",  ParamValue =model.OutsideAllowance,DBType=DbType.Decimal},
                new QueryParamObj { ParamName = "@NetSalary",  ParamValue =model.NetSalary,DBType=DbType.Decimal},


                new QueryParamObj { ParamName = "@BankName",  ParamValue =model.BankName},
                new QueryParamObj { ParamName = "@EmployeeAccount",  ParamValue =model.EmployeeAccount},
                new QueryParamObj { ParamName = "@SalaryCategory",  ParamValue =model.SalaryCategory},
               
            };
            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
        }

        private void SaveBatchFileOtherDetails(EmployeeBatchUploadModel model, Database db, DbTransaction trans, int pk)
        {
            if (model == null)
                return;
            DbCommand templateCommand = db.GetStoredProcCommand("Employee_SaveOtherDetailsBatch");
            var queryParamList = new QueryParamList
            {
                new QueryParamObj { ParamName = "@EmployeeId",  ParamValue =pk,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@LeavePolicyCode",  ParamValue =model.LeavePolicyCode},
                new QueryParamObj { ParamName = "@LeaveEntitlement",  ParamValue =model.LeaveEntitlement,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@AirTicketEntitlementTotalMonth",  ParamValue =model.AirTicketEntitlementTotalMonth,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@HiredThrough",  ParamValue =model.HiredThrough,DBType=DbType.String},
                new QueryParamObj { ParamName = "@ContractPeriodYear",  ParamValue =model.ContractPeriodYear,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@LaborContract",  ParamValue =model.LaborContract,DBType=DbType.String},
                new QueryParamObj { ParamName = "@ContractIssueDate",  ParamValue =model.ContractIssueDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@ContractExpiryDate",  ParamValue =model.ContractExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@CompanyID",  ParamValue =model.CompanyID,DBType=DbType.String},
                new QueryParamObj { ParamName = "@MotherTongue",  ParamValue =model.MotherTongue,DBType=DbType.String},
                new QueryParamObj { ParamName = "@MaritalStatus",  ParamValue =model.MaritalStatus,DBType=DbType.String},


                new QueryParamObj { ParamName = "@ChildrenNo",  ParamValue =model.ChildrenNo,DBType=DbType.Int32},
                new QueryParamObj { ParamName = "@Religion",  ParamValue =model.Religion},
                new QueryParamObj { ParamName = "@PreviousCompany",  ParamValue =model.PreviousCompany},
                new QueryParamObj { ParamName = "@Country",  ParamValue =model.Country},

                new QueryParamObj { ParamName = "@HomeAirport",  ParamValue =model.HomeAirport,DBType=DbType.String},
                new QueryParamObj { ParamName = "@CompanyEmailID",  ParamValue =model.CompanyEmailID},
                new QueryParamObj { ParamName = "@EmailIDs",  ParamValue =model.EmailIDs},
                new QueryParamObj { ParamName = "@EmployeeGroup",  ParamValue =model.EmployeeGroup},

                new QueryParamObj { ParamName = "@EmployeeSubGroup",  ParamValue =model.EmployeeSubGroup,DBType=DbType.String},
                new QueryParamObj { ParamName = "@EmployeeOfTheMonth",  ParamValue =model.EmployeeOfTheMonth},
                new QueryParamObj { ParamName = "@UpdaLicense",  ParamValue =model.UpdaLicense},
                new QueryParamObj { ParamName = "@Registration",  ParamValue =model.Registration},

                 new QueryParamObj { ParamName = "@Grade",  ParamValue =model.Grade,DBType=DbType.String},
                new QueryParamObj { ParamName = "@UdpaExpiryDate",  ParamValue =model.UdpaExpiryDate,DBType=DbType.DateTime},
                new QueryParamObj { ParamName = "@Remarks",  ParamValue =model.Remarks}

            };
            DBExecStoredProcInTran(db, templateCommand, queryParamList, trans);
        }

        public EmployeeDocExpiryDaysModel GetEmployeeDocExpiry(int? companyId)
        {
            const string sql = @"declare @qidExpired int=0, @qidExpired90Days int=0, @qidExpired60Days int=0, @qidExpired30Days int=0
                                    declare @visaExpired int=0, @visaExpired90Days int=0, @visaExpired60Days int=0, @visaExpired30Days int=0
                                    declare @passportExpired int=0, @passportExpired90Days int=0, @passportExpired60Days int=0, @passportExpired30Days int=0
                                    declare @healthExpired int=0, @healthExpired90Days int=0, @healthExpired60Days int=0, @healthExpired30Days int=0

                                    select @qidExpired=COUNT(e.Id)
                                    from Employee e
                                    where e.QIDExpiryDate is not null and e.QIDExpiryDate<GETDATE() and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @qidExpired90Days=COUNT(e.Id)
                                    from Employee e
                                    where e.QIDExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.QIDExpiryDate)>=61 and DATEDIFF(DAY, GETDATE(), e.QIDExpiryDate)<=90
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @qidExpired60Days=COUNT(e.Id)
                                    from Employee e
                                    where e.QIDExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.QIDExpiryDate)>=31 and DATEDIFF(DAY, GETDATE(), e.QIDExpiryDate)<=60
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @qidExpired30Days=COUNT(e.Id)
                                    from Employee e
                                    where e.QIDExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.QIDExpiryDate)>=0 and DATEDIFF(DAY, GETDATE(), e.QIDExpiryDate)<=30
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @visaExpired=COUNT(e.Id)
                                    from Employee e
                                    where e.VisaExpirayDate is not null and e.VisaExpirayDate<GETDATE()
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)
                                    select @visaExpired90Days=COUNT(e.Id)
                                    from Employee e
                                    where e.VisaExpirayDate is not null and DATEDIFF(DAY, GETDATE(), e.VisaExpirayDate)>=61 and DATEDIFF(DAY, GETDATE(), e.VisaExpirayDate)<=90
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @visaExpired60Days=COUNT(e.Id)
                                    from Employee e
                                    where e.VisaExpirayDate is not null and DATEDIFF(DAY, GETDATE(), e.VisaExpirayDate)>=31 and DATEDIFF(DAY, GETDATE(), e.VisaExpirayDate)<=60
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @visaExpired30Days=COUNT(e.Id)
                                    from Employee e
                                    where e.VisaExpirayDate is not null and DATEDIFF(DAY, GETDATE(), e.VisaExpirayDate)>=0 and DATEDIFF(DAY, GETDATE(), e.VisaExpirayDate)<=30
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @passportExpired=COUNT(e.Id)
                                    from Employee e
                                    where e.PassportExpiryDate is not null and e.PassportExpiryDate<GETDATE()
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @passportExpired90Days=COUNT(e.Id)
                                    from Employee e
                                    where e.PassportExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.PassportExpiryDate)>=61 and DATEDIFF(DAY, GETDATE(), e.PassportExpiryDate)<=90
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @passportExpired60Days=COUNT(e.Id)
                                    from Employee e
                                    where e.PassportExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.PassportExpiryDate)>=31 and DATEDIFF(DAY, GETDATE(), e.PassportExpiryDate)<=60
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @passportExpired30Days=COUNT(e.Id)
                                    from Employee e
                                    where e.PassportExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.PassportExpiryDate)>=0 and DATEDIFF(DAY, GETDATE(), e.PassportExpiryDate)<=30
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @healthExpired=COUNT(e.Id)
                                    from Employee e
                                    where e.HealthCardExpiryDate is not null and e.HealthCardExpiryDate<GETDATE()
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @healthExpired90Days=COUNT(e.Id)
                                    from Employee e
                                    where e.HealthCardExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.HealthCardExpiryDate)>=61 and DATEDIFF(DAY, GETDATE(), e.HealthCardExpiryDate)<=90
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @healthExpired60Days=COUNT(e.Id)
                                    from Employee e
                                    where e.HealthCardExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.HealthCardExpiryDate)>=31 and DATEDIFF(DAY, GETDATE(), e.HealthCardExpiryDate)<=60
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @healthExpired30Days=COUNT(e.Id)
                                    from Employee e
                                    where e.HealthCardExpiryDate is not null and DATEDIFF(DAY, GETDATE(), e.HealthCardExpiryDate)>=0 and DATEDIFF(DAY, GETDATE(), e.HealthCardExpiryDate)<=30
                                     and (@companyId is null or e.WorkingCompanyId=@companyId)

                                    select @qidExpired QidExpiry,@qidExpired90Days QidExpiry90Days,@qidExpired60Days QidExpiry60Days,@qidExpired30Days QidExpiry30Days,
                                    @visaExpired VisaExpiry,@visaExpired30Days VisaExpiry30Days,@visaExpired60Days VisaExpiry60Days,@visaExpired90Days VisaExpiry90Days,
                                    @passportExpired PassportExpiry,@passportExpired30Days PassportExpiry30Days,@passportExpired60Days PassportExpiry60Days,@passportExpired90Days PassportExpiry90Days,
                                    @healthExpired HealthExpiry,@healthExpired30Days HealthExpiry30Days,@healthExpired60Days HealthExpiry60Days,@healthExpired90Days HealthExpiry90Days";
            var queryParamList = new QueryParamList
               {
                    new QueryParamObj { ParamName = "@companyId", ParamValue =companyId ,DBType = DbType.Int32},
                };
            var result = ExecuteDBQuery(sql, queryParamList, EmployeeMapper.ToDocExpiryModel);
            return result.FirstOrDefault();
        }
    }



    public static class EmployeeMapper
    {
        public static List<CompanyModel> ToCompanyModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<CompanyModel>();

            while (readers.Read())
            {
                var model = new CompanyModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    CompanyName = Convert.IsDBNull(readers["CompanyName"]) ? string.Empty : Convert.ToString(readers["CompanyName"]),
                    Address = Convert.IsDBNull(readers["Address"]) ? string.Empty : Convert.ToString(readers["Address"]),
                    PhoneNumber = Convert.IsDBNull(readers["PhoneNumber"]) ? string.Empty : Convert.ToString(readers["PhoneNumber"]),
                    ImageFileName = Convert.IsDBNull(readers["ImageFileName"]) ? string.Empty : Convert.ToString(readers["ImageFileName"]),
                    ImageFileId = Convert.IsDBNull(readers["ImageFileId"]) ? string.Empty : Convert.ToString(readers["ImageFileId"]),
                    CreatedById = Convert.IsDBNull(readers["CreatedById"]) ? string.Empty : Convert.ToString(readers["CreatedById"]),
                    CreatedDate = Convert.IsDBNull(readers["CreatedDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["CreatedDate"]),
                    IsActive = Convert.IsDBNull(readers["IsActive"]) ? false : Convert.ToBoolean(readers["IsActive"]),
                    CompanyAdminName = Convert.IsDBNull(readers["CompanyAdminName"]) ? string.Empty : Convert.ToString(readers["CompanyAdminName"]),
                    CompanyAdminEmail = Convert.IsDBNull(readers["CompanyAdminEmail"]) ? string.Empty : Convert.ToString(readers["CompanyAdminEmail"]),
                    CompanyAdminLoginID = Convert.IsDBNull(readers["CompanyAdminLoginID"]) ? string.Empty : Convert.ToString(readers["CompanyAdminLoginID"]),
                };

                models.Add(model);
            }

            return models;
        }
        public static List<EmployeeCreateModel> ToModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeCreateModel>();

            while (readers.Read())
            {
                var model = new EmployeeCreateModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    EmployeeCode = Convert.ToString(readers["EmployeeCode"]),
                    EmployeeName = Convert.ToString(readers["EmployeeName"]),
                    StatusId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                    DepartmentId = Convert.IsDBNull(readers["DepartmentId"]) ? (int?)null : Convert.ToInt32(readers["DepartmentId"]),
                    DesignationId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                    MobileNo = Convert.IsDBNull(readers["MobileNo"]) ? string.Empty : Convert.ToString(readers["MobileNo"]),
                    
                };

                models.Add(model);
            }

            return models;
        }
        
        public static List<EmployeeExportModel> ToListModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeExportModel>();
            int i = 1;
            while (readers.Read())
            {
                var model = new EmployeeExportModel
                {
                    Id = Convert.ToInt64(readers["Id"]),
                    SerialNo = i,
                    EmployeeCode = Convert.ToString(readers["EmployeeCode"]),
                    EmployeeName = Convert.ToString(readers["EmployeeName"]),
                    StatusId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                    LoginID = Convert.IsDBNull(readers["LoginID"]) ? string.Empty : Convert.ToString(readers["LoginID"]),
                    CompanyAdminEmail = Convert.IsDBNull(readers["CompanyAdminEmail"]) ? string.Empty : Convert.ToString(readers["CompanyAdminEmail"]),
                    Department = Convert.IsDBNull(readers["DepartmentName"]) ? string.Empty : Convert.ToString(readers["DepartmentName"]),
                    Designation = Convert.IsDBNull(readers["DesignationName"]) ? string.Empty : Convert.ToString(readers["DesignationName"]),
                    MobileNo = Convert.IsDBNull(readers["MobileNo"]) ? string.Empty : Convert.ToString(readers["MobileNo"]),
                    JoiningDate = Convert.IsDBNull(readers["DateOfJoining"]) ? (DateTime?)null : Convert.ToDateTime(readers["DateOfJoining"]),
                    Nationality = Convert.IsDBNull(readers["NationalityName"]) ? string.Empty : Convert.ToString(readers["NationalityName"]),
                    DateOfBirth = Convert.IsDBNull(readers["DateOfBirth"]) ? (DateTime?)null : Convert.ToDateTime(readers["DateOfBirth"]),
                    Gender = Convert.IsDBNull(readers["GenderName"]) ? string.Empty : Convert.ToString(readers["GenderName"]),
                    WorkingCompany = Convert.IsDBNull(readers["WorkingCompanyName"]) ? string.Empty : Convert.ToString(readers["WorkingCompanyName"]),
                    WorkLocation = Convert.IsDBNull(readers["WorkLocationName"]) ? string.Empty : Convert.ToString(readers["WorkLocationName"]),
                    HealthCardNo = Convert.IsDBNull(readers["HealthCardNo"]) ? string.Empty : Convert.ToString(readers["HealthCardNo"]),
                    HealthCardExpiry = Convert.IsDBNull(readers["HealthCardExpiryDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["HealthCardExpiryDate"]),
                    Insurance = Convert.IsDBNull(readers["Insurance"]) ? string.Empty : Convert.ToString(readers["Insurance"]),
                    InsuranceExpiryDate = Convert.IsDBNull(readers["InsuranceExpirayDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["InsuranceExpirayDate"]),
                    BasicPay = Convert.IsDBNull(readers["BasicPay"]) ? (Decimal?)null : Convert.ToDecimal(readers["BasicPay"]),
                    Housing = Convert.IsDBNull(readers["Housing"]) ? (Decimal?)null : Convert.ToDecimal(readers["Housing"]),
                    Transport = Convert.IsDBNull(readers["Transport"]) ? (Decimal?)null : Convert.ToDecimal(readers["Transport"]),
                    Telephone = Convert.IsDBNull(readers["Telephone"]) ? (Decimal?)null : Convert.ToDecimal(readers["Telephone"]),
                    FoodAllowance = Convert.IsDBNull(readers["FoodAllowance"]) ? (Decimal?)null : Convert.ToDecimal(readers["FoodAllowance"]),
                    OtherAllowances = Convert.IsDBNull(readers["OtherAllowancce"]) ? (Decimal?)null : Convert.ToDecimal(readers["OtherAllowancce"]),
                    NetSalary = Convert.IsDBNull(readers["NetSalary"]) ? (Decimal?)null : Convert.ToDecimal(readers["NetSalary"]),
                    ContractIssueDate = Convert.IsDBNull(readers["ContractIssueDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["ContractIssueDate"]),
                    ContractExpiryDate = Convert.IsDBNull(readers["ContractExpiryDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["ContractExpiryDate"]),
                    CompanyID = Convert.IsDBNull(readers["CompanyID"]) ? string.Empty : Convert.ToString(readers["CompanyID"]),
                    SalaryCategory = Convert.IsDBNull(readers["SalaryCategory"]) ? string.Empty : Convert.ToString(readers["SalaryCategory"]),
                    BankName = Convert.IsDBNull(readers["BankName"]) ? string.Empty : Convert.ToString(readers["BankName"]),
                    EmployeeAccount = Convert.IsDBNull(readers["EmployeeAccount"]) ? string.Empty : Convert.ToString(readers["EmployeeAccount"]),
                    MotherTongue = Convert.IsDBNull(readers["MotherTongue"]) ? string.Empty : Convert.ToString(readers["MotherTongue"]),
                    MaritalStatus = Convert.IsDBNull(readers["MaritalStatusName"]) ? string.Empty : Convert.ToString(readers["MaritalStatusName"]),
                    Religion = Convert.IsDBNull(readers["ReligionName"]) ? string.Empty : Convert.ToString(readers["ReligionName"]),
                    PreviousCompany = Convert.IsDBNull(readers["PreviousCompany"]) ? string.Empty : Convert.ToString(readers["PreviousCompany"]),
                    Country = Convert.IsDBNull(readers["CountryName"]) ? string.Empty : Convert.ToString(readers["CountryName"]),
                    EmailIDs = Convert.IsDBNull(readers["EmailIDs"]) ? string.Empty : Convert.ToString(readers["EmailIDs"]),
                    Grade = Convert.IsDBNull(readers["Grade"]) ? string.Empty : Convert.ToString(readers["Grade"]),
                    LastWorkngDate = Convert.IsDBNull(readers["LastWorkingDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["LastWorkingDate"]),
                    MaritalStatusId = Convert.IsDBNull(readers["MaritalStatusId"]) ? (int?)null : Convert.ToInt32(readers["MaritalStatusId"]),
                    Remarks = Convert.IsDBNull(readers["Remarks"]) ? string.Empty : Convert.ToString(readers["Remarks"]),
                    LeavePolicyId = Convert.IsDBNull(readers["LeavePolicyId"]) ? (int?)null : Convert.ToInt32(readers["LeavePolicyId"]),
                    ReligionId = Convert.IsDBNull(readers["ReligionId"]) ? (int?)null : Convert.ToInt32(readers["ReligionId"]),
                    CountryId = Convert.IsDBNull(readers["CountryId"]) ? (int?)null : Convert.ToInt32(readers["CountryId"]),
                    DesignationId = Convert.IsDBNull(readers["DesignationId"]) ? (int?)null : Convert.ToInt32(readers["DesignationId"]),
                    DepartmentId = Convert.IsDBNull(readers["DepartmentId"]) ? (int?)null : Convert.ToInt32(readers["DepartmentId"]),
                    ProjectId = Convert.IsDBNull(readers["ProjectId"]) ? (int?)null : Convert.ToInt32(readers["ProjectId"]),
                    NationalityId = Convert.IsDBNull(readers["NationalityId"]) ? (int?)null : Convert.ToInt32(readers["NationalityId"]),
                    GenderId = Convert.IsDBNull(readers["GenderId"]) ? (int?)null : Convert.ToInt32(readers["GenderId"]),
                    WorkingCompanyId = Convert.IsDBNull(readers["WorkingCompanyId"]) ? (int?)null : Convert.ToInt32(readers["WorkingCompanyId"]),
                    WorkLocationId = Convert.IsDBNull(readers["WorkLocationId"]) ? (int?)null : Convert.ToInt32(readers["WorkLocationId"]),
                    ImageFileName = Convert.IsDBNull(readers["ImageFileName"]) ? string.Empty : Convert.ToString(readers["ImageFileName"]),
                    IsDeleteable = Convert.IsDBNull(readers["IsDeleteable"]) ? (bool?)null : Convert.ToBoolean(readers["IsDeleteable"]),
                };
                i += 1;
                model.ImagePath = Constants.LocalFilePath + model.ImageFileName;
                models.Add(model);
            }

            return models;
        }
        public static List<EmployeeExportModel> ToEmpListModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeExportModel>();
            while (readers.Read())
            {
                var model = new EmployeeExportModel
                {   
                    EmployeeName = Convert.ToString(readers["EmployeeName"]),
                    Department = Convert.IsDBNull(readers["DepartmentName"]) ? string.Empty : Convert.ToString(readers["DepartmentName"]),
                    Designation = Convert.IsDBNull(readers["DesignationName"]) ? string.Empty : Convert.ToString(readers["DesignationName"]),
                    PortalUserId = Convert.IsDBNull(readers["PortalUserId"]) ? string.Empty : Convert.ToString(readers["PortalUserId"]),
                    WorkingCompanyId = Convert.IsDBNull(readers["WorkingCompanyId"]) ? (int?)null : Convert.ToInt32(readers["WorkingCompanyId"]),

                };
                models.Add(model);
            }

            return models;
        }
        public static List<EmployeeDocExpiryDaysModel> ToDocExpiryModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeDocExpiryDaysModel>();
            while (readers.Read())
            {
                var model = new EmployeeDocExpiryDaysModel
                {
                    QidExpiry = Convert.ToInt32(readers["QidExpiry"]),
                    QidExpiry30Days = Convert.ToInt32(readers["QidExpiry30Days"]),
                    QidExpiry60Days = Convert.ToInt32(readers["QidExpiry60Days"]),
                    QidExpiry90Days = Convert.ToInt32(readers["QidExpiry90Days"]),
                    VisaExpiry = Convert.ToInt32(readers["VisaExpiry"]),
                    VisaExpiry30Days = Convert.ToInt32(readers["VisaExpiry30Days"]),
                    VisaExpiry60Days = Convert.ToInt32(readers["VisaExpiry60Days"]),
                    VisaExpiry90Days = Convert.ToInt32(readers["VisaExpiry90Days"]),
                    PassportExpiry = Convert.ToInt32(readers["PassportExpiry"]),
                    PassportExpiry30Days = Convert.ToInt32(readers["PassportExpiry30Days"]),
                    PassportExpiry60Days = Convert.ToInt32(readers["PassportExpiry60Days"]),
                    PassportExpiry90Days = Convert.ToInt32(readers["PassportExpiry90Days"]),
                    HealthExpiry = Convert.ToInt32(readers["HealthExpiry"]),
                    HealthExpiry30Days = Convert.ToInt32(readers["HealthExpiry30Days"]),
                    HealthExpiry60Days = Convert.ToInt32(readers["HealthExpiry60Days"]),
                    HealthExpiry90Days = Convert.ToInt32(readers["HealthExpiry90Days"]),
                };
                models.Add(model);
            }

            return models;
        }
        public static List<EmployeeDetailsModel> ToDetailsModel(DbDataReader readers)
        {
            if (readers == null)
                return null;
            var models = new List<EmployeeDetailsModel>();
            int i = 1;
            while (readers.Read())
            {
                var model = new EmployeeDetailsModel
                {
                    Id = Convert.ToInt32(readers["Id"]),
                    SerialNo = i,
                    EmployeeCode = Convert.ToString(readers["EmployeeCode"]),
                    EmployeeName = Convert.ToString(readers["EmployeeName"]),
                    EmployeeStatusId = Convert.IsDBNull(readers["StatusId"]) ? (int?)null : Convert.ToInt32(readers["StatusId"]),
                    LoginID = Convert.IsDBNull(readers["LoginID"]) ? string.Empty : Convert.ToString(readers["LoginID"]),
                    Department = Convert.IsDBNull(readers["DepartmentName"]) ? string.Empty : Convert.ToString(readers["DepartmentName"]),
                    Designation = Convert.IsDBNull(readers["DesignationName"]) ? string.Empty : Convert.ToString(readers["DesignationName"]),
                    MobileNo = Convert.IsDBNull(readers["MobileNo"]) ? string.Empty : Convert.ToString(readers["MobileNo"]),
                    LineManager = Convert.IsDBNull(readers["LineManagerName"]) ? string.Empty : Convert.ToString(readers["LineManagerName"]),
                    DepartmentManager= Convert.IsDBNull(readers["DepartmentManagerName"]) ? string.Empty : Convert.ToString(readers["DepartmentManagerName"]),
                    EmployeeHrDirector = Convert.IsDBNull(readers["HrDirectorName"]) ? string.Empty : Convert.ToString(readers["HrDirectorName"]),
                    Project = Convert.IsDBNull(readers["ProjectName"]) ? string.Empty : Convert.ToString(readers["ProjectName"]),
                    JoiningDateVw = Convert.IsDBNull(readers["DateOfJoining"]) ? string.Empty : Convert.ToDateTime(readers["DateOfJoining"]).ToString(Constants.DateFormat),
                    Nationality = Convert.IsDBNull(readers["NationalityName"]) ? string.Empty : Convert.ToString(readers["NationalityName"]),
                    DateOfBirthVw = Convert.IsDBNull(readers["DateOfBirth"]) ? string.Empty : Convert.ToDateTime(readers["DateOfBirth"]).ToString(Constants.DateFormat),
                    Gender = Convert.IsDBNull(readers["GenderName"]) ? string.Empty : Convert.ToString(readers["GenderName"]),
                    PassportNo = Convert.IsDBNull(readers["PassportNo"]) ? string.Empty : Convert.ToString(readers["PassportNo"]),
                    PassportIssueDateVw = Convert.IsDBNull(readers["PassportIssueDate"]) ? string.Empty : Convert.ToDateTime(readers["PassportIssueDate"]).ToString(Constants.DateFormat),
                    PassportExpiryDateVw = Convert.IsDBNull(readers["PassportExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["PassportExpiryDate"]).ToString(Constants.DateFormat),
                    VisaExpiryDateVw = Convert.IsDBNull(readers["VisaExpirayDate"]) ? string.Empty : Convert.ToDateTime(readers["VisaExpirayDate"]).ToString(Constants.DateFormat),
                    //PassportInfoUpdateOnQidCost= Convert.IsDBNull(readers["PassportInfoUpdateOnQidCost"]) ? (Decimal?)null : Convert.ToDecimal(readers["PassportInfoUpdateOnQidCost"]),
                    QID = Convert.IsDBNull(readers["QID"]) ? string.Empty : Convert.ToString(readers["QID"]),
                    QIDExpiryDateVw = Convert.IsDBNull(readers["QIDExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["QIDExpiryDate"]).ToString(Constants.DateFormat),
                    //RpFines= Convert.IsDBNull(readers["RpFines"]) ? (Decimal?)null : Convert.ToDecimal(readers["RpFines"]),
                    //ResidencePermitIssuanceCost = Convert.IsDBNull(readers["ResidencePermitIssuanceCost"]) ? (Decimal?)null : Convert.ToDecimal(readers["ResidencePermitIssuanceCost"]),
                    WorkingCompany = Convert.IsDBNull(readers["WorkingCompanyName"]) ? string.Empty : Convert.ToString(readers["WorkingCompanyName"]),
                    SponsorshipId = Convert.IsDBNull(readers["SponsorshipId"]) ? (int?)null : Convert.ToInt32(readers["SponsorshipId"]),
                    VisaNo = Convert.IsDBNull(readers["VisaNo"]) ? string.Empty : Convert.ToString(readers["VisaNo"]),
                    VisaExpirayDate= Convert.IsDBNull(readers["VisaExpirayDate"]) ? (DateTime?)null : Convert.ToDateTime(readers["VisaExpirayDate"]),
                    WorkLocation = Convert.IsDBNull(readers["WorkLocationName"]) ? string.Empty : Convert.ToString(readers["WorkLocationName"]),
                    CompanyAccomodation = Convert.IsDBNull(readers["CompanyAccomodation"]) ? string.Empty : Convert.ToString(readers["CompanyAccomodation"]),
                    HealthCardNo = Convert.IsDBNull(readers["HealthCardNo"]) ? string.Empty : Convert.ToString(readers["HealthCardNo"]),
                    HealthCardExpiryVw = Convert.IsDBNull(readers["HealthCardExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["HealthCardExpiryDate"]).ToString(Constants.DateFormat),
                    //HealthCardIssuanceCost= Convert.IsDBNull(readers["HealthCardIssuanceCost"]) ? (Decimal?)null: Convert.ToDecimal(readers["HealthCardIssuanceCost"]),
                    Insurance = Convert.IsDBNull(readers["Insurance"]) ? string.Empty : Convert.ToString(readers["Insurance"]),
                    InsuranceExpiryDateVw = Convert.IsDBNull(readers["InsuranceExpirayDate"]) ? string.Empty : Convert.ToDateTime(readers["InsuranceExpirayDate"]).ToString(Constants.DateFormat),
                    FoodHandling = Convert.IsDBNull(readers["FoodHandling"]) ? string.Empty : Convert.ToString(readers["FoodHandling"]),
                    FoodHandlingIssueDateVw = Convert.IsDBNull(readers["FoodHandlingIssueDate"]) ? string.Empty : Convert.ToDateTime(readers["FoodHandlingIssueDate"]).ToString(Constants.DateFormat),
                    FoodhandlingExpiryDateVw = Convert.IsDBNull(readers["FoodhandlingExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["FoodhandlingExpiryDate"]).ToString(Constants.DateFormat),
                    BasicPay = Convert.IsDBNull(readers["BasicPay"]) ? (Decimal?)null : Convert.ToDecimal(readers["BasicPay"]),                  
                    Housing = Convert.IsDBNull(readers["Housing"]) ? (Decimal?)null : Convert.ToDecimal(readers["Housing"]),
                    Transport = Convert.IsDBNull(readers["Transport"]) ? (Decimal?)null : Convert.ToDecimal(readers["Transport"]),
                    Telephone = Convert.IsDBNull(readers["Telephone"]) ? (Decimal?)null : Convert.ToDecimal(readers["Telephone"]),
                    FoodAllowance = Convert.IsDBNull(readers["FoodAllowance"]) ? (Decimal?)null : Convert.ToDecimal(readers["FoodAllowance"]),
                    OtherAllowances = Convert.IsDBNull(readers["OtherAllowancce"]) ? (Decimal?)null : Convert.ToDecimal(readers["OtherAllowancce"]),
                    TeamLeaderAllowance = Convert.IsDBNull(readers["TeamLeadAllowance"]) ? (Decimal?)null : Convert.ToDecimal(readers["TeamLeadAllowance"]),
                    CityCompensatoryAllowance = Convert.IsDBNull(readers["CityCompensatoryAllowance"]) ? (Decimal?)null : Convert.ToDecimal(readers["CityCompensatoryAllowance"]),
                    PersonalAllowance = Convert.IsDBNull(readers["PersonalAllowance"]) ? (Decimal?)null : Convert.ToDecimal(readers["PersonalAllowance"]),
                    OutSideAllowance = Convert.IsDBNull(readers["OutSideAllowance"]) ? (Decimal?)null : Convert.ToDecimal(readers["OutSideAllowance"]),
                    NetSalary = Convert.IsDBNull(readers["NetSalary"]) ? (Decimal?)null : Convert.ToDecimal(readers["NetSalary"]),
                    LeavePolicy = Convert.IsDBNull(readers["LeavePolicy"]) ? string.Empty : Convert.ToString(readers["LeavePolicy"]),
                    LeavePolicyCode = Convert.IsDBNull(readers["LeavePolicyCode"]) ? string.Empty : Convert.ToString(readers["LeavePolicyCode"]),
                    LeaveEntitlement = Convert.IsDBNull(readers["LeaveEntitlement"]) ? string.Empty : Convert.ToString(readers["LeaveEntitlement"]),
                    AirTicketEntitlement = Convert.IsDBNull(readers["AirTicketEntitlementTotalMonth"]) ? string.Empty : Convert.ToString(readers["AirTicketEntitlementTotalMonth"]),
                    HiredThrough = Convert.IsDBNull(readers["HiredThrough"]) ? string.Empty : Convert.ToString(readers["HiredThrough"]),
                    ContractPeriod = Convert.IsDBNull(readers["ContractPeriodYear"]) ? string.Empty : Convert.ToString(readers["ContractPeriodYear"]),
                    LaborContract = Convert.IsDBNull(readers["LaborContract"]) ? string.Empty : Convert.ToString(readers["LaborContract"]),
                   // NewContractCost = Convert.IsDBNull(readers["NewContractCost"]) ? string.Empty : Convert.ToString(readers["NewContractCost"]),
                    ContractIssueDateVw = Convert.IsDBNull(readers["ContractIssueDate"]) ? string.Empty : Convert.ToDateTime(readers["ContractIssueDate"]).ToString(Constants.DateFormat),
                    ContractExpiryDateVw = Convert.IsDBNull(readers["ContractExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["ContractExpiryDate"]).ToString(Constants.DateFormat),
                    CompanyID = Convert.IsDBNull(readers["CompanyID"]) ? string.Empty : Convert.ToString(readers["CompanyID"]),
                    SalaryCategory = Convert.IsDBNull(readers["SalaryCategory"]) ? string.Empty : Convert.ToString(readers["SalaryCategory"]),
                    BankName = Convert.IsDBNull(readers["BankName"]) ? string.Empty : Convert.ToString(readers["BankName"]),
                    EmployeeAccount = Convert.IsDBNull(readers["EmployeeAccount"]) ? string.Empty : Convert.ToString(readers["EmployeeAccount"]),
                    MotherTongueId = Convert.IsDBNull(readers["MotherTongueId"]) ? (int?)null : Convert.ToInt32(readers["MotherTongueId"]),
                    MaritalStatus = Convert.IsDBNull(readers["MaritalStatusName"]) ? string.Empty : Convert.ToString(readers["MaritalStatusName"]),
                    ChildrenNo = Convert.IsDBNull(readers["ChildrenNo"]) ? (int?)null : Convert.ToInt32(readers["ChildrenNo"]),
                    Religion = Convert.IsDBNull(readers["ReligionName"]) ? string.Empty : Convert.ToString(readers["ReligionName"]),
                    PreviousCompany = Convert.IsDBNull(readers["PreviousCompany"]) ? string.Empty : Convert.ToString(readers["PreviousCompany"]),
                    Country = Convert.IsDBNull(readers["CountryName"]) ? string.Empty : Convert.ToString(readers["CountryName"]),
                    HomeAirportId = Convert.IsDBNull(readers["HomeAirportId"]) ? (int?)null : Convert.ToInt32(readers["HomeAirportId"]),
                    CompanyEmailID = Convert.IsDBNull(readers["CompanyEmailID"]) ? string.Empty : Convert.ToString(readers["CompanyEmailID"]),
                    EmailIDs = Convert.IsDBNull(readers["EmailIDs"]) ? string.Empty : Convert.ToString(readers["EmailIDs"]),
                    EmployeeGroupId = Convert.IsDBNull(readers["EmployeeGroupId"]) ? (int?)null : Convert.ToInt32(readers["EmployeeGroupId"]),
                    EmployeeSubGroup = Convert.IsDBNull(readers["EmployeeSubGroup"]) ? string.Empty : Convert.ToString(readers["EmployeeSubGroup"]),
                    EmployeeOfTheMonth = Convert.IsDBNull(readers["EmployeeOfTheMonth"]) ? string.Empty : Convert.ToString(readers["EmployeeOfTheMonth"]),
                    EpdaLicense = Convert.IsDBNull(readers["UpdaLicense"]) ? string.Empty : Convert.ToString(readers["UpdaLicense"]),
                    Registration = Convert.IsDBNull(readers["Registration"]) ? string.Empty : Convert.ToString(readers["Registration"]),
                    Grade = Convert.IsDBNull(readers["Grade"]) ? string.Empty : Convert.ToString(readers["Grade"]),
                    UpdaExpiryDateVw = Convert.IsDBNull(readers["UdpaExpiryDate"]) ? string.Empty : Convert.ToDateTime(readers["UdpaExpiryDate"]).ToString(Constants.DateFormat),
                    LastWorkngDateVw = Convert.IsDBNull(readers["LastWorkingDate"]) ? string.Empty : Convert.ToDateTime(readers["LastWorkingDate"]).ToString(Constants.DateFormat),
                    MaritalStatusId = Convert.IsDBNull(readers["MaritalStatusId"]) ? (int?)null : Convert.ToInt32(readers["MaritalStatusId"]),
                    Remarks = Convert.IsDBNull(readers["Remarks"]) ? string.Empty : Convert.ToString(readers["Remarks"]),
                    LeavePolicyId = Convert.IsDBNull(readers["LeavePolicyId"]) ? (int?)null : Convert.ToInt32(readers["LeavePolicyId"]),
                    ReligionId = Convert.IsDBNull(readers["ReligionId"]) ? (int?)null : Convert.ToInt32(readers["ReligionId"]),
                    CountryId = Convert.IsDBNull(readers["CountryId"]) ? (int?)null : Convert.ToInt32(readers["CountryId"]),
                    DesignationId = Convert.IsDBNull(readers["DesignationId"]) ? (int?)null : Convert.ToInt32(readers["DesignationId"]),
                    DepartmentId = Convert.IsDBNull(readers["DepartmentId"]) ? (int?)null : Convert.ToInt32(readers["DepartmentId"]),
                    ProjectId = Convert.IsDBNull(readers["ProjectId"]) ? (int?)null : Convert.ToInt32(readers["ProjectId"]),
                    NationalityId = Convert.IsDBNull(readers["NationalityId"]) ? (int?)null : Convert.ToInt32(readers["NationalityId"]),
                    GenderId = Convert.IsDBNull(readers["GenderId"]) ? (int?)null : Convert.ToInt32(readers["GenderId"]),
                    WorkingCompanyId = Convert.IsDBNull(readers["WorkingCompanyId"]) ? (int?)null : Convert.ToInt32(readers["WorkingCompanyId"]),
                    WorkLocationId = Convert.IsDBNull(readers["WorkLocationId"]) ? (int?)null : Convert.ToInt32(readers["WorkLocationId"]),
                    HasAccessQrCodeScan = Convert.IsDBNull(readers["HasAccessQrCodeScan"]) ? (bool?)null : Convert.ToBoolean(readers["HasAccessQrCodeScan"]),
                    ImageFileName = Convert.IsDBNull(readers["ImageFileName"]) ? string.Empty : Convert.ToString(readers["ImageFileName"]),
                    

                };
                i += 1;
                model.ImagePath = Constants.LocalFilePath + model.ImageFileName;
                models.Add(model);
            }

            return models;
        }
    }
}
