using DaewooLMS.Models;
using System.Collections.Generic;

namespace DaewooLMS.Models.ViewModel
{
    public class AddNewPolicyVM
    {
        public NewHrPolicies NewHrPolicies { get; set; }
        public AddPolicies AddPolicies { get; set; }
        public List<AddPolicies> newaddPolicies { get; set; }
    }
}
