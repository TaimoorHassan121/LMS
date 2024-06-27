using DaewooLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Models
{
    public class QuizQuestionAnswer
    {
        [Key]
        public int QuizID { get; set; }
        public int QuizQno { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int DepartmentID { get; set; }
        [ForeignKey("DepartmentID")]
        public virtual Department Departments { get; set; }
        public bool QStatus { get; set; }
        public DateTime QuizDate { get; set; }
    }
}
