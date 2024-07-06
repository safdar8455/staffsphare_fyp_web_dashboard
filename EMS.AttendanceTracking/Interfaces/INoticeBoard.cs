using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Interfaces
{
    public interface INoticeBoard
    {
        ResponseModel Save(NoticeBoardModel model);
        List<NoticeBoardModel> GetAll(int companyId);
        NoticeBoardModel Get(string id);
        ResponseModel Delete(string id);
        ResponseModel UploadImageFile(NoticeBoardModel model);
    }
}
