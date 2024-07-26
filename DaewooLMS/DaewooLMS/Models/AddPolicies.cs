using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaewooLMS.Models
{
    public class AddPolicies
    {
        [Key]
        public int PolicyHeadingID { get; set; }
        public string? PolicyHeading { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        [MaxLength]
        public string? PolicyDetail { get; set; }
        public int PolicyID { get; set; }

    }
}
