using LMS.Models;
using System.Collections.Generic;

namespace LMSProject.Models.ViewModel
{
    public class Emp_ChatVM
    {
        public List<Emp_Chat_M> Emp_Chat_M { get; set; }
        public List<Emp_Chat_Reply> Emp_Chat_Reply { get; set; }
        public Employee employee { get; set; }      
        public QuizDepartment Departments { get; set;}
    }
}
