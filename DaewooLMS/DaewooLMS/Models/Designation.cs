using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Models
{
    public class Designation
    {
        [Key]
        public int Designation_ID { get; set; }
        public string Designation_Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime DesgDate { get; set; }
    }
}
