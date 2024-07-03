using System.Collections.Generic;

namespace DaewooLMS.Models.ViewModel
{
    public class ProfileVM
    {
        public Employee Employee { get; set; }

        public List<myNotes> myNotes { get; set; }
    }
}
