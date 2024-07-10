using DaewooLMS.Data;
using DaewooLMS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "UserCookieScheme", Roles = "Employee")]
    public class LiberaryPDFController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LiberaryPDFController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PDFDetail(string deprtname)
        {
            LiberaryDetail liberaryDetail = new LiberaryDetail();
            var LiberaryDb = _context.LibraryDatas.ToList();
            var depart = _context.Departments.Where(a => a.DepartmentName == deprtname).FirstOrDefault();

            var PDFLibrary = LiberaryDb.Where(a => a.DepartmentID == depart.DepartmentID && a.Trade == "LiberaryPDF").ToList();

            liberaryDetail.LiberaryDataList = PDFLibrary;
            liberaryDetail.LiberaryDataName = depart.DepartmentName;



            return View(liberaryDetail);
        }

        public ActionResult GetFileUrl(string fileName)
        {
            string fileUrl = Url.Content($"~/CoursePDF/{fileName}");
            return Json(new { url = fileUrl });
        }
    }
}
