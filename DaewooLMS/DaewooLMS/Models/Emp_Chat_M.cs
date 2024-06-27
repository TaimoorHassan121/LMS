using System;
using System.ComponentModel.DataAnnotations;

namespace DaewooLMS.Models
{
    public class Emp_Chat_M
    {
        [Key]
        public int ChatID { get; set; }
        public int Emp_ID { get; set; }
        [StringLength(50)]
        public string Emp_Name { get; set; }
        [StringLength(25)]
        public string Emp_Designation { get; set; }
        public string message { get; set; }
        public string Emp_Pic { get; set; }
        public bool IsSeen { get; set; } = false;
        public bool Status { get; set; }
        public DateTime MsgDate { get; set; }
    }
}
