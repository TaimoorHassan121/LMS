using System.ComponentModel.DataAnnotations;
using System;

namespace DaewooLMS.Models
{
    public class Event_Logs
    {
        [Key]
        public int EventLogID { get; set; }
        public int EventID { get; set; }
        [Display(Name = "Event Title")]
        public string Event_Title { get; set; }
        public string Objactive { get; set; }
        public string Participent { get; set; }
        public string Event_PIC { get; set; }
        [Display(Name = "Event Start Date Time")]
        public DateTime start_DateTime { get; set; }
        [Display(Name = "Event End Date Time")]
        public DateTime End_DateTime { get; set; }
        public DateTime Add_Update_DateTime { get; set; }
        public string CRUD_Status { get; set; }
        public long Authorize_Person { get; set; }
        public bool status { get; set; }
    }
}
