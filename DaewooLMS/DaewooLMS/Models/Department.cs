using System.ComponentModel.DataAnnotations;
using System;

namespace DaewooLMS.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }
    }
}
