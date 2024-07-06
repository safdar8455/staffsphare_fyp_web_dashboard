using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Services
{
	public class NoticeBoardBusiness
	{
		private readonly INoticeBoard _NoticeBoard;

		public NoticeBoardBusiness()
		{
			_NoticeBoard = AttendanceUnityMapper.GetInstance<INoticeBoard>();
		}
		public ResponseModel Save(NoticeBoardModel model)
		{
			return _NoticeBoard.Save(model);
		}
		public ResponseModel UploadImageFile(NoticeBoardModel model)
		{
			return _NoticeBoard.UploadImageFile(model);
		}
		public IEnumerable<NoticeBoardModel> GetAll(int companyId)
		{
			var list = _NoticeBoard.GetAll(companyId);
			return list;
		}

		public NoticeBoardModel Get(string id)
		{
			var list = _NoticeBoard.Get(id);
			return list;
		}

		public ResponseModel Delete(string id)
		{
			return _NoticeBoard.Delete(id);
		}
	}
}
