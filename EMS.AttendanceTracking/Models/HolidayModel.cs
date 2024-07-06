using Ems.BusinessTracker.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Ems.AttendanceTracking.Models
{

    public enum HolidayTypeEnum
    {
        [Description("Full Day")]
        FullDay = 1,
        [Description("Half Day")]
        HalfDay = 2
    }

    public class HolidayModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public DateTime? HolidayDate { get; set; }
        public int? TypeId { get; set; }
        public int? CompanyId { get; set; }
        public string HolidayDateStr { get; set; }
        public string HolidayDateVw
        {
            get { return HolidayDate.HasValue ? HolidayDate.Value.ToString(Constants.DateLongFormat) : string.Empty; }
        }

        public string HolidayType
        {
            get
            {
                return TypeId.HasValue? EnumUtility.GetDescriptionFromEnumValue((HolidayTypeEnum)TypeId):string.Empty;
            }
        }
    }
}
