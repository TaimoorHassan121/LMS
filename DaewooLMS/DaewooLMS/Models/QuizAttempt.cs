using System;
using System.ComponentModel.DataAnnotations;

namespace DaewooLMS.Models
{
    public class QuizAttempt
    {
        [Key]
        public int QuizAtmp_ID { get; set; }
        public long Emp_ID { get; set; }
        public string Emp_Department { get; set; }
        public int Quiz_Attempts { get; set; }
        public string? Score { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public string Status { get; set; }
    }
}
