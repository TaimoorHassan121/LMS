using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Models
{
    public class QuizOptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OptID { get; set; }
        public string Options { get; set; }
        public int QuizNo { get; set; }
        public int Department { get; set; }
        public DateTime OptDate { get; set; }
        public int OptionNo { get; set; }
    }
}
