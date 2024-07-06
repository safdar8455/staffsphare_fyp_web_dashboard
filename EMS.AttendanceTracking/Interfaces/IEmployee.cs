using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface IEmployee
    {
        List<CompanyModel> GetCompanyList(); 
        List<EmployeeExportModel> GetAll();
        List<EmployeeExportModel> GetAllEmp();
        EmployeeDocExpiryDaysModel GetEmployeeDocExpiry(int? companyId);
        ResponseModel UpdateEmployeeImage(EmployeeDetailsModel model);
        List<EmployeeDetailsModel> GetEmployeeDetails(long? employeeId);
        ResponseModel Delete(int id);
        ResponseModel UpdateEmployeeBatchFile(EmployeeBatchUploadModel model);
        ResponseModel SaveEmployee(EmployeeDetailsModel model);
    }
}
