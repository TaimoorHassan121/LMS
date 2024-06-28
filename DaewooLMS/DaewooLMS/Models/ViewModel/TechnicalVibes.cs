using LMS.Models;
using System.Collections.Generic;

namespace LMSProject.Models.ViewModel
{
    public class TechnicalVibes
    {
        public List<LibraryData> LiberaryDataList { get; set; }
        public List<Videos> Videos { get; set; }
        public List<QuizQuestionAnswer> QuizQuestions { get; set; }

        public List<questionData> questionDatas { get; set; }= new List<questionData>();
   
        public string DepartmentName { get; set; }


        public class questionData
        {
            public int numb { get; set; }
            public string question { get; set; }
            public string answer { get; set; }
            public List<string> options { get; set; }
        }
    }
}
