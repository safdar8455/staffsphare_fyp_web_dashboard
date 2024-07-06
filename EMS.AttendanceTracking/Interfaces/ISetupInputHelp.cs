using Ems.BusinessTracker.Common.Models;
using System.Collections.Generic;
using Ems.AttendanceTracking.Models;
namespace Ems.AttendanceTracking.Interfaces
{
	public interface ISetupInputHelp
	{
		ResponseModel Save(SetupInputHelpModel model);
		List<SetupInputHelpModel> GetAll();
		ResponseModel DeleteInputHelp(int id);
		ResponseModel UpdateQrCode(EmployeeExportModel qrCodeModel);
	}
}
