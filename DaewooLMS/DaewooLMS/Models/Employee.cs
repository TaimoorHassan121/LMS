using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using DaewooLMS.Models;

namespace DaewooLMS.Models
{
    public class Employee
    {
        [Key]
        public int UserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public string IdentityUserId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        public int Emp_ID { get; set; }
        public string Emp_Name { get; set; }
        public string Emp_Pic { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
       
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }
        [ForeignKey("DepartmentID")]
        public virtual Department Department { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Mobile_No { get; set; }
        [DataType(DataType.Password)]
        public string User_Passward { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public DateTime Emp_Date { get; set; }
    }
}
