using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using DaewooLMS.Models;

namespace DaewooLMS.Models
{
    public class Videos
    {
        [Key]
        public int VideoID { get; set; }
        public string Video_Name { get; set; }
        public string Video_Link { get; set; }
        public string Trade { get; set; }
        public int DepartmentID { get; set; }
        [ForeignKey("DepartmentID")]
        public virtual Department Departments { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
