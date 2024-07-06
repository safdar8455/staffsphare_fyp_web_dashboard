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
    public class EmployeeBusiness
	{
		private readonly IEmployee _Employee;
        private readonly IDesignation _Designation;
        private readonly IDepartment _department;
        private readonly ISetupInputHelp _setupInputHelp;
        private string _UserId;
		public EmployeeBusiness(string userId)
		{
            _UserId = userId;
			_Employee = AttendanceUnityMapper.GetInstance<IEmployee>();
            _department = AttendanceUnityMapper.GetInstance<IDepartment>();
            _Designation = AttendanceUnityMapper.GetInstance<IDesignation>();
            _setupInputHelp = AttendanceUnityMapper.GetInstance<ISetupInputHelp>();
        }
		public ResponseModel Save(EmployeeDetailsModel model)
		{
            model.EmployeeStatusId = model.Id == 0 ? (int)EmployeeStatus.Active : model.EmployeeStatusId;
            ConvertStringToDateTime(model);
			return _Employee.SaveEmployee(model);
		}

        private void ConvertStringToDateTime(EmployeeDetailsModel model)
        {
            model.JoiningDate = string.IsNullOrEmpty(model.JoiningDateVw) ? (DateTime?)null : Convert.ToDateTime(model.JoiningDateVw);
            model.DateOfBirth = string.IsNullOrEmpty(model.DateOfBirthVw) ? (DateTime?)null : Convert.ToDateTime(model.DateOfBirthVw);
            model.PassportIssueDate = string.IsNullOrEmpty(model.PassportIssueDateVw) ? (DateTime?)null : Convert.ToDateTime(model.PassportIssueDateVw);
            model.PassportExpiryDate = string.IsNullOrEmpty(model.PassportExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.PassportExpiryDateVw);
            model.QIDExpiryDate = string.IsNullOrEmpty(model.QIDExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.QIDExpiryDateVw);
            model.VisaExpirayDate = string.IsNullOrEmpty(model.VisaExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.VisaExpiryDateVw);
            model.InsuranceExpiryDate = string.IsNullOrEmpty(model.InsuranceExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.InsuranceExpiryDateVw);
            model.FoodhandlingExpiryDate = string.IsNullOrEmpty(model.FoodhandlingExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.FoodhandlingExpiryDateVw);
            model.FoodHandlingIssueDate = string.IsNullOrEmpty(model.FoodHandlingIssueDateVw) ? (DateTime?)null : Convert.ToDateTime(model.FoodHandlingIssueDateVw);
            model.ContractIssueDate = string.IsNullOrEmpty(model.ContractIssueDateVw) ? (DateTime?)null : Convert.ToDateTime(model.ContractIssueDateVw);
            model.ContractExpiryDate = string.IsNullOrEmpty(model.ContractExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.ContractExpiryDateVw);
            model.UpdaExpiryDate = string.IsNullOrEmpty(model.UpdaExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.UpdaExpiryDateVw);
            model.LastWorkngDate = string.IsNullOrEmpty(model.LastWorkngDateVw) ? (DateTime?)null : Convert.ToDateTime(model.LastWorkngDateVw);
            model.HealthCardExpiryDate = string.IsNullOrEmpty(model.HealthCardExpiryVw) ? (DateTime?)null : Convert.ToDateTime(model.HealthCardExpiryVw);
            model.PassportExpiryDate = string.IsNullOrEmpty(model.PassportExpiryDateVw) ? (DateTime?)null : Convert.ToDateTime(model.PassportExpiryDateVw);
        }

        public IEnumerable<EmployeeExportModel> GetAll()
		{
			var list = _Employee.GetAll();
			return list;
		}

        public List<EmployeeExportModel> GetEmployeeExportList()
        {
            var list = _Employee.GetAll();
            return list;
        }
        public ResponseModel UpdateEmployeeImage(EmployeeDetailsModel model)
        {
            return _Employee.UpdateEmployeeImage(model);
        }
        public EmployeeDetailsModel GetEmployeeDetails(long employeeId,int? companyId)
        {
            var list = _Employee.GetEmployeeDetails(employeeId).ToList();
            var result = list.Any()?list.FirstOrDefault():new EmployeeDetailsModel();
            LoadEmployeeDropDownData(result,companyId);
            return result;
        }
        private void LoadEmployeeDropDownData(EmployeeDetailsModel model, int? companyId)
        {
            model.EmployeeStatusList = Enum.GetValues(typeof(EmployeeStatus)).Cast<EmployeeStatus>().Select(c => new NameIdPairModel
            {
                Id = (int)c,
                Name = EnumUtility.GetDescriptionFromEnumValue(c)
            }).ToList();
            model.DepartmentList = _department.GetAll().Where(x=>x.CompanyId==companyId).Select(x => new NameIdPairModel
            {
                Id = x.Id,
                Name = x.DepartmentName
            }).ToList();
            model.DesignationList = _Designation.GetAll(companyId??0).Select(x => new NameIdPairModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            var inputHelp = _setupInputHelp.GetAll();
            if (inputHelp.Any())
            {
                model.NationalityList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.Nationality).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.GenderList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.Gender).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
               
                model.SponsorCompanyList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.SponsorCompany).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.WorkingLocationList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.WorkingLocation).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.MotherTongueList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.MotherTongue).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.ReligionList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.Religion).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.CountryList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.Country).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.HomeAirportList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.HomeAireport).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.EmployeeGroupList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.EmployeeGroup).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.ProjectList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.Project).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
                model.MaritalStatusList = inputHelp.Where(c => c.InputHelpTypeId == (int)InputHelpType.MaritalStatus).Select(c => new NameIdPairModel { Name = c.Name, Id = c.Id }).OrderBy(c => c.Name).ToList();
            }

        }
        public ResponseModel Delete(int id)
		{
			return _Employee.Delete(id);
		}

        public ResponseModel UpdateEmployeeBatchFile(string[] columns,int? cId)
        {
            var model = PrepareEmployeeProfileData(columns,cId);
            model.ActionById = _UserId;
            var response = _Employee.UpdateEmployeeBatchFile(model);

            return response;
        }

        private EmployeeBatchUploadModel PrepareEmployeeProfileData(string[] columns,int? cId)
        {
            var model = new EmployeeBatchUploadModel
            {
                EmployeeCode = string.IsNullOrEmpty(columns[0].Trim()) ? null : columns[0].Trim(),
                EmployeeName = string.IsNullOrEmpty(columns[1].Trim()) ? null : columns[1].Trim(),
                Designation = string.IsNullOrEmpty(columns[2].Trim()) ? null : columns[2].Trim(),
                Department = string.IsNullOrEmpty(columns[3].Trim()) ? null : columns[3].Trim(),
                DateOfJoining = string.IsNullOrEmpty(columns[4].Trim()) ? (DateTime?)null : Convert.ToDateTime(columns[4].Trim()),
                Nationality = string.IsNullOrEmpty(columns[5].Trim()) ? null : columns[5].Trim(),
                DateOfBirth = string.IsNullOrEmpty(columns[6].Trim()) ? (DateTime?)null : Convert.ToDateTime(columns[6].Trim()),
                Gender = string.IsNullOrEmpty(columns[7].Trim()) ? null : columns[7].Trim(),
                WorkingCompanyId =cId,
                WorkLocation = string.IsNullOrEmpty(columns[8].Trim()) ? null : columns[8].Trim(),
                HealthCardNo = string.IsNullOrEmpty(columns[9].Trim()) ? null : columns[9].Trim(),
                HealthCardExpiryDate = string.IsNullOrEmpty(columns[10].Trim()) ? (DateTime?)null : Convert.ToDateTime(columns[10].Trim()),
                MobileNo = string.IsNullOrEmpty(columns[11].Trim()) ? null : columns[11].Trim(),
                Insurance = string.IsNullOrEmpty(columns[12].Trim()) ? null : columns[12].Trim(),
                InsuranceExpirayDate = string.IsNullOrEmpty(columns[13].Trim()) ? (DateTime?)null : Convert.ToDateTime(columns[13].Trim()),
                BasicPay = string.IsNullOrEmpty(columns[14].Trim()) ? 0 : Convert.ToDecimal(columns[14].Trim()),
                Housing = string.IsNullOrEmpty(columns[15].Trim()) ? 0 : Convert.ToDecimal(columns[15].Trim()),
                Transport = string.IsNullOrEmpty(columns[16].Trim()) ? (Decimal?)null : Convert.ToDecimal(columns[16].Trim()),
                Telephone = string.IsNullOrEmpty(columns[17].Trim()) ? (Decimal?)null : Convert.ToDecimal(columns[17].Trim()),
                FoodAllowance = string.IsNullOrEmpty(columns[18].Trim()) ? (Decimal?)null : Convert.ToDecimal(columns[18].Trim()),
                OtherAllowancce = string.IsNullOrEmpty(columns[19].Trim()) ? (Decimal?)null : Convert.ToDecimal(columns[19].Trim()),
                NetSalary = string.IsNullOrEmpty(columns[20].Trim()) ? (Decimal?)null : Convert.ToDecimal(columns[20].Trim()),
                ContractIssueDate = string.IsNullOrEmpty(columns[21].Trim()) ? (DateTime?)null : Convert.ToDateTime(columns[21].Trim()),
                ContractExpiryDate = string.IsNullOrEmpty(columns[22].Trim()) ? (DateTime?)null : Convert.ToDateTime(columns[22].Trim()),
                BankName = string.IsNullOrEmpty(columns[23].Trim()) ? null : columns[23].Trim(),
                EmployeeAccount = string.IsNullOrEmpty(columns[24].Trim()) ? null : columns[24].Trim(),
                MotherTongue = string.IsNullOrEmpty(columns[25].Trim()) ? null : columns[25].Trim(),
                MaritalStatus = string.IsNullOrEmpty(columns[26].Trim()) ? null : columns[26].Trim(),
                Religion = string.IsNullOrEmpty(columns[27].Trim()) ? null : columns[27].Trim(),
                Country = string.IsNullOrEmpty(columns[28].Trim()) ? null : columns[28].Trim(),
                EmailIDs = string.IsNullOrEmpty(columns[29].Trim()) ? null : columns[29].Trim(),
                Grade = string.IsNullOrEmpty(columns[30].Trim()) ? null : columns[30].Trim(),
                LastWorkingDate = string.IsNullOrEmpty(columns[31].Trim()) ? (DateTime?)null : Convert.ToDateTime(columns[31].Trim()),
                Status = string.IsNullOrEmpty(columns[32].Trim()) ? null : columns[32].Trim(),
                Remarks = string.IsNullOrEmpty(columns[33].Trim()) ? null : columns[33].Trim()
            };
            return model;
        }

    }
}
