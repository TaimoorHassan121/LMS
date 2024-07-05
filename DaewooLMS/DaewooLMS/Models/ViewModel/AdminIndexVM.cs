using System.Collections.Generic;

namespace DaewooLMS.Models.ViewModel
{
    public class AdminIndexVM
    {

        public List<Employee> employees { get; set; }
        public List<Emp_Chat_M> chatMaster { get; set; }
        public List<Emp_Chat_Reply> chatReply { get; set; }
        public List<Support> support { get; set; }
        public List<Events> events { get; set; }


    }
}
