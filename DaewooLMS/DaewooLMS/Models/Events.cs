using System;
using System.ComponentModel.DataAnnotations;

namespace DaewooLMS.Models
{
    public class Events
    {
        [Key]
        public int EventID { get; set; }
        [Display(Name = "Event Title")]
        public string  Event_Title { get; set; }
        public string Objactive { get; set; }
        public string  Participent { get; set; }
        public string Event_PIC { get; set; }
        [Display(Name = "Event Start Date Time")]
        public DateTime start_DateTime { get; set; }
        [Display(Name = "Event End Date Time")]
        public DateTime End_DateTime { get; set; }
        public DateTime Add_DateTime { get; set; }= DateTime.Now;
        public bool status { get; set; }
    }
}
