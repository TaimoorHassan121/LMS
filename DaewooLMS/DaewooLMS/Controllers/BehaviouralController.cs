using DaewooLMS.Data;
using DaewooLMS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "UserCookieScheme", Roles = "Employee")]
    public class BehaviouralController : Controller
    {

        private readonly ApplicationDbContext _context;

        public BehaviouralController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> BehaviouralTraining(string deprtname)
        {

            BehaviouralVibes behaviouralVibes = new BehaviouralVibes();

            var LiberaryDb = _context.LibraryDatas.ToList();
            var depart = await _context.Departments.Where(a => a.DepartmentName == deprtname).FirstOrDefaultAsync();
            var PDFTechnical = LiberaryDb.Where(a => a.DepartmentID == depart.DepartmentID && a.Trade == "BehaviouralPDF").ToList();
            var VideoTechnical = _context.Videos.Where(a => a.DepartmentID == depart.DepartmentID && a.Trade == "BehaviouralVideo").ToList();


            behaviouralVibes.LiberaryDataList = PDFTechnical;
            behaviouralVibes.Videos = VideoTechnical;
            behaviouralVibes.DepartmentName = depart.DepartmentName;


            return View(behaviouralVibes);
        }
    }
}
