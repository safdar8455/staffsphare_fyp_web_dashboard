using System;
using Ems.BusinessTracker.Common;
namespace Ems.AttendanceTracking.Models
{
    public class NoticeBoardModel
    {
        public string Id { get; set; }
        public int NoticeId { get; set; }
        public int? CompanyId { get; set; }
        public string Details { get; set; }
        public DateTime? PostingDate { get; set; }
        public string ImageFileName { get; set; }
        public string ImagePath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string PostingDateVw { get { return PostingDate.HasValue ?PostingDate.Value.ToZoneTime().ToString(Constants.DateFormat) : string.Empty; } }
        public LocalDocumentModel AttachedDocument { get; set; }
    }
}
