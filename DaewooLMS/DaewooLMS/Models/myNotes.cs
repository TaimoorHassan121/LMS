using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace DaewooLMS.Models
{
    public class myNotes
    {
        [Key]
        public int myNoteId { get; set; }
        public string myNote { get; set;}
        public long  Emp_Id { get; set; }
        public bool status { get; set; }
        public DateTime Date { get; set; }
    }
}
