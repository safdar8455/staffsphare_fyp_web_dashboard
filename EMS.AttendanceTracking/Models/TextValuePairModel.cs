
namespace Ems.AttendanceTracking.Models
{
    public class TextValuePairModel
    {
        public string label { get; set; }
        public string value { get; set; }
    }
    public class TextValuePairModelEmp
    {
        public string label { get; set; }
        public string id { get; set; }
    }
    //public class NameIdPairModel
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public bool IsDeleted { get; set; }
    //    public string Description { get; set; }
    //    public int SerialNo { get; set; }
    //}

    public class LabelIdModel
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class NameValueModel
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string Code { get; set; }
    }
    public class NameValueModelLong
    {
        public long Value { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class NameCountPairModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class NameCountPairModelString
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Count { get; set; }
    }
}
