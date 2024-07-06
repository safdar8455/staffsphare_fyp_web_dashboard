using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using System.Linq;
using Ems.AttendanceTracking.Interfaces;
using Ems.AttendanceTracking.Mappers;
using Ems.AttendanceTracking.Models;

namespace Ems.AttendanceTracking.Services
{
	public class SetupInputHelpBusiness
	{

		private readonly ISetupInputHelp _setupInputHelp;

		public SetupInputHelpBusiness()
		{
			_setupInputHelp = AttendanceUnityMapper.GetInstance<ISetupInputHelp>();
		}
		public ResponseModel Save(SetupInputHelpModel model)
		{
			var list = _setupInputHelp.GetAll().Where(m => m.InputHelpTypeId == model.InputHelpTypeId && m.Name.ToLower() == model.Name.ToLower()).ToList();
			var message = "Name already exists";

			if ((list.Count == 0) || (list.Count == 1 && model.IsActive != list[0].IsActive && model.Id != 0))
			{
				return _setupInputHelp.Save(model);
			}
			else
			{
				return new ResponseModel { Success = false, Message = message };
			}
		}
		public IEnumerable<SetupInputHelpModel> GetAll()
		{
            var list = _setupInputHelp.GetAll();
		    return list;
		}
		public ResponseModel UpdateQrCode(EmployeeExportModel qrCodeModel)
		{
			var response = _setupInputHelp.UpdateQrCode(qrCodeModel);
			return response;
		}
		public ResponseModel DeleteInputHelp(int id)
		{
			return _setupInputHelp.DeleteInputHelp(id);
		}
	}
}
