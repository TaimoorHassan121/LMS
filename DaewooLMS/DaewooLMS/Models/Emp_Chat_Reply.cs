using System;
using System.ComponentModel.DataAnnotations;

namespace DaewooLMS.Models
{
    public class Emp_Chat_Reply
    {
        [Key]
        public int Chat_ReplyID { get; set; }
        public int ChatM_ID { get; set; }
        public int Emp_ChatM_ID { get; set; }
        public int Emp_Reply_ID { get; set; }
        public string Reply_Message { get; set; }
        public string Emp_Name { get; set; }
        public string Emp_Pic_Reply { get; set; }
        public bool IsSeen { get; set; } = false;
        public bool Status { get; set; }
        public DateTime MsgDate { get; set; }
    }
}
