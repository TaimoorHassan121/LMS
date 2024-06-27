using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaewooLMS.Models
{
    public class LibraryData
    {
        [Key]
        public int PDFID { get; set; }
        public string Pdf_Name { get; set; }
        public string Pdf_File { get; set; }
        public string Trade { get; set; }
        public int DepartmentID { get; set; }
        [ForeignKey("DepartmentID")]
        public virtual Department Departments { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

    }
}
