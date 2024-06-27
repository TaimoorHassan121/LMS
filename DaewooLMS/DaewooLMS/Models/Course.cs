using DaewooLMS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Models
{
    public class Course
    {
        [Key]
        public int Course_ID { get; set; }
        public string Course_Name { get; set; }
        public string Course_Type { get; set; }
        public int DepartmentID { get; set; }
        [ForeignKey("DepartmentID")]
        public virtual Department Departments { get; set; }
        public string  PDFFile { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public DateTime CourseDate { get; set; }
    }
}
