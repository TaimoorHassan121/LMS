using System;
using System.ComponentModel.DataAnnotations;

namespace DaewooLMS.Models
{
    public class NewHrPolicies
    {
        [Key]
        public int PolicyID { get; set; }     
        public string? PolicyName { get; set; } 
        public DateTime PolicyDate { get; set; }
        public bool PolicyStatus { get; set; }



    }
}
