using Ems.BusinessTracker.Common;

namespace Ems.AttendanceTracking.Models
{
	public class SetupInputHelpModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int InputHelpTypeId { get; set; }
		public bool IsActive { get; set; }

		public string InputHelpTypeName
		{
			get
			{
				return InputHelpTypeId > 0 ? EnumUtility.GetDescriptionFromEnumValue((InputHelpType)InputHelpTypeId) : string.Empty;
			}
		}

		public string StatusName
		{
			get
			{
				return IsActive?"Active":"In Active";
			}
		}
	}
}
