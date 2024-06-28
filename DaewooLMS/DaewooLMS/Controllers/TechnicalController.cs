using DaewooLMS.Data;
using DaewooLMS.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using static DaewooLMS.Models.ViewModel.TechnicalVibes;
using System.Threading.Tasks;
using System.Linq;

namespace DaewooLMS.Controllers
{
    public class TechnicalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TechnicalController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> TechnicalTraining(string deprtname)
        {

            TechnicalVibes technicalVibes = new TechnicalVibes();

            var LiberaryDb = _context.LibraryDatas.ToList();
            var depart = _context.Departments.Where(a => a.DepartmentName == deprtname).FirstOrDefault();
            var PDFTechnical = LiberaryDb.Where(a => a.DepartmentID == depart.DepartmentID && a.Trade == "LiberaryPDF").ToList();
            var VideoTechnical = _context.Videos.Where(a => a.DepartmentID == depart.DepartmentID && a.Trade == "TechnicalVideo").ToList();
            var quizMain = _context.QuizQuestionAnswer.Where(a => a.DepartmentID == depart.DepartmentID).ToList();

            technicalVibes.LiberaryDataList = PDFTechnical;
            technicalVibes.Videos = VideoTechnical;
            technicalVibes.DepartmentName = depart.DepartmentName;

            foreach (var item in quizMain)
            {
                questionData question = new questionData();
                question.numb = item.QuizID;
                question.question = item.Question;
                question.answer = item.Answer;

                var optionDb = _context.QuizOptions.Where(a => a.QuizNo == item.QuizID).Select(a => a.Options).ToList();
                question.options = optionDb;
                //foreach (var option in optionDb)
                //{
                //    question.options.Add(option);
                //}
                technicalVibes.questionDatas.Add(question);
            }
            technicalVibes.questionDatas = technicalVibes.questionDatas.ToList();
            return View(technicalVibes);
        }
    }
}
