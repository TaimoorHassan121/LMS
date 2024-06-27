using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Models
{
    public class Terminal
    {
        [Key]
        public int Terminal_ID { get; set; }
        public string  Trrminal_Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime TrmnlDate { get; set; }
    }
}
