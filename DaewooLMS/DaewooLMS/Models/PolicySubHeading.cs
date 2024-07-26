using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DaewooLMS.Models
{
    public class PolicySubHeading
    {
        [Key]
        public int SubHeadingID { get; set; }
        public int Main_HeadingID { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string SubHeading_Title { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        [MaxLength]
        public string Sub_Detail { get; set; }
        public bool Status { get; set; }
    }
}
