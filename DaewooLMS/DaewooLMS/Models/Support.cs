using System;

namespace DaewooLMS.Models
{
    public class Support
    {
        public int SupportID { get; set; }
        public string Emp_Name { get; set; }
        public long Emp_ID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public DateTime MsgDate { get; set; }
    }
}
