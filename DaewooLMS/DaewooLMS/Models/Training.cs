using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Models
{
    public class Training
    {
        [Key]
        public int Training_ID { get; set; }
        public string Training_Name { get; set; }
        public string Training_Type { get; set; }
        public string Training_Domain { get; set; }
        public string Training_purpose { get; set; }
        public DateTime Training_Date { get; set; }
        public DateTime Training_SatrtDate { get; set; }
        public DateTime Training_EndDate { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
    }
}
