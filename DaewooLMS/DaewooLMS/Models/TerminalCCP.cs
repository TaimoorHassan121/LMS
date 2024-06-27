using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Models
{
    public class TerminalCCP
    {
        [Key]
        public int CCP_ID { get; set; }
        public string CCP_Name { get; set; }       
        public int Terminal_ID { get; set; }
        [ForeignKey("Terminal_ID")]
        public virtual Terminal Terminal { get; set; }
        public bool IsActive { get; set; }
        public DateTime CCPDate { get; set; }
    }
}
