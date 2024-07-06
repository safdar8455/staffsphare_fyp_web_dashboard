
namespace Ems.BusinessTracker.Common.Models
{
    public class CodeNamePairModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class NameIdPairModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public int SerialNo { get; set; }
    }
    public class NameIdPairModelLong
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public int SerialNo { get; set; }
    }
    public class LabelIdModel
    {
        public int id { get; set; }
        public string label { get; set; }
    }
}
