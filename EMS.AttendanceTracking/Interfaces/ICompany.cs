using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface ICompany
    {
        List<Company> GetCompanyList();
        ResponseModel Create(Company model);
        ResponseModel AddOrUpdateCompany(Company model);
        ResponseModel DeleteCompanyAttachments(int id);
        List<AttachmentModel> GetCompanyAttachments(int id);
        ResponseModel Update(Company model);
        ResponseModel UpdateLogo(int id, string fileId, string fileName);
        ResponseModel Delete(int id);
    }
}