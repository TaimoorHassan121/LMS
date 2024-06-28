using DaewooLMS.Data;
using DaewooLMS.Models;
using LMSProject.Services.Interface;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DaewooLMS.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DaewooLMS.Models.ViewModel;

namespace DaewooLMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHTMLtoWord _htmltoword;

        public HomeController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RegisterModel> logger, IEmailSender emailSender, IHTMLtoWord hTMLtoWord)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
            _htmltoword = hTMLtoWord;
        }

        [BindProperty]
        public string ReturnUrl { get; set; }

        private List<AuthenticationScheme> ExternalLogins;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DaewooLMS()
        {
            return View();
        }
        public IActionResult AdminIndex()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)

        {
            returnUrl = returnUrl ?? Url.Content("~/Home/Index");
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Employee employee, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(employee.Emp_ID.ToString());
            var emp = _context.Employees.Where(a => a.Emp_ID == employee.Emp_ID).SingleOrDefault();

            if (user != null)
            {
                var role = await _userManager.GetRolesAsync(user);
                if (role.ElementAt(0) == "Employee")
                {
                    var result = await _signInManager.PasswordSignInAsync(employee.Emp_ID.ToString(), employee.User_Passward, employee.IsActive, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        await CreateCookies(emp);
                        _logger.LogInformation("User logged in.");
                        return RedirectToAction("Index", "Home");
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View();
                    }
                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync("UserCookies");
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Login));
            }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["QuizDepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int id, string name, string mob, string pwd, int deprt, string design, string gender, IFormFile profilepic, Employee employee, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var useremployee = _context.Employees.Where(a => a.Emp_ID == id).SingleOrDefault();

            if (useremployee != null)
            {
                TempData["message"] = "Employee Already Available";
                return RedirectToAction("Register");
            }
            if (ModelState.IsValid)
            {
                employee.Emp_ID = id;
                employee.Emp_Name = name;
                employee.Mobile_No = mob;
                employee.User_Passward = pwd;
                employee.DepartmentID = deprt;
                employee.Designation = design;
                employee.Gender = gender;
                employee.Emp_Date = DateTime.Now;
                employee.Status = true;
                var user = new IdentityUser { UserName = employee.Emp_ID.ToString(), Email = employee.Emp_ID.ToString(), PhoneNumber = employee.Mobile_No, EmailConfirmed = true };
                employee.IdentityUser = user;


                if (profilepic != null)
                {
                    var image = ContentDispositionHeaderValue.Parse(profilepic.ContentDisposition).FileName.Trim();
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", profilepic.FileName);
                    using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                    {
                        profilepic.CopyTo(stream);
                    }
                    employee.Emp_Pic = profilepic.FileName;
                }
                var result = await _userManager.CreateAsync(user, employee.User_Passward);
                if (result.Succeeded)
                {
                    //var role = await _roleManager.FindByNameAsync("Employee");

                    await _roleManager.CreateAsync(new IdentityRole("Employee"));
                    await _userManager.AddToRoleAsync(user, "Employee");
                    employee.IsActive = true;

                    _logger.LogInformation("User created a new account with password.");
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return Ok(employee);

                }
            }


            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }
        public IActionResult Digital_Liberary()
        {
            return View();
        }
        public IActionResult Videos()
        {
            return View();
        }
        public IActionResult CoursesDetail()
        {
            return View();
        }
        public IActionResult VideosDetails()
        {
            return View();
        }
        public IActionResult TechnicalTraining()
        {
            return View();
        }
        public IActionResult BehaviourlTraining()
        {
            return View();
        }

        public IActionResult BehaviourlTrainingDemo()
        {
            return View();
        }
        public IActionResult TechnicalTrainingDemo()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DaewooCommunity()
        {

            long Emp_ID = HttpContext.User.Claims
           .Where(x => x.Type == "Emp_ID")
           .Select(x => Convert.ToInt64(x.Value))
           .FirstOrDefault();
            var imageUrl = User.Claims.Where(a => a.Type == "Emp_Pic")
               .Select(x => x.Value)
               .FirstOrDefault();


            Emp_ChatVM emp_Chat = new Emp_ChatVM();

            var emp_Chat_M = _context.Emp_Chat_M.Where(a => a.IsSeen == true).OrderByDescending(a => a.ChatID).ToList();
            var chat_reply = _context.Emp_Chat_Replies.Where(a => a.IsSeen == true).ToList();
            var employee = _context.Employees.Where(a => a.Emp_ID == Emp_ID).SingleOrDefault();
            var QuizDepartment = _context.Departments.Where(a => a.DepartmentID == employee.DepartmentID).SingleOrDefault();
            emp_Chat.Emp_Chat_M = emp_Chat_M;
            emp_Chat.employee = employee;
            emp_Chat.Emp_Chat_Reply = chat_reply;
            emp_Chat.Departments = QuizDepartment;


            ViewData["QuizDepartmentID"] = new SelectList(_context.Departments, "QuizDepartmentID", "QuizDepartmentName");

            return View(emp_Chat);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DaewooCommunityADD(int id, string name, string designation, string message, string emp_pic, Emp_Chat_M chat)
        {
            chat.Emp_ID = id;
            chat.Emp_Name = name;
            chat.Emp_Designation = designation;
            chat.message = message;
            chat.Emp_Pic = emp_pic;
            chat.Status = true;
            chat.MsgDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(chat);
                await _context.SaveChangesAsync();
                return RedirectToAction("DaewooCommunity");
            }
            return View();


        }
        public async Task<IActionResult> DaewooCommunityReply(int CurrentEmpId, int Emp_ChatID_M, int Chat_ID_M, string message, string Emp_Pic, string Emp_Name, Emp_Chat_Reply chatReply)
        {

            chatReply.Emp_Reply_ID = CurrentEmpId;
            chatReply.Emp_ChatM_ID = Emp_ChatID_M;
            chatReply.ChatM_ID = Chat_ID_M;
            chatReply.Reply_Message = message;
            chatReply.Emp_Pic_Reply = Emp_Pic;
            chatReply.Emp_Name = Emp_Name;
            chatReply.IsSeen = false;
            chatReply.Status = true;
            chatReply.MsgDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(chatReply);
                await _context.SaveChangesAsync();
                return RedirectToAction("DaewooCommunity");
            }
            return View();


        }
        public IActionResult Events()
        {
            return View();
        }
        public IActionResult Support()
        {
            return View();
        }
        public IActionResult HRHUB()
        {
            return View();
        }
        public IActionResult EmployeeWelBeing()
        {
            return View();
        }

        [HttpGet]
        public IActionResult HRDevelopPolicies()
        {
            ViewData["Policy"] = new SelectList(_context.NewHrPolicies, "PolicyID", "PolicyName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HRDevelopPolicies(AddNewPolicyVM policyVM)
        {
            var hrPolicy = _context.NewHrPolicies.FirstOrDefault();
            var addPolicy = _context.AddPolicies.FirstOrDefault();

            hrPolicy = policyVM.NewHrPolicies;
            addPolicy = policyVM.AddPolicies;
            var oldPolicy = await _context.NewHrPolicies.Where(a => a.PolicyName == hrPolicy.PolicyName).SingleOrDefaultAsync();

            if (oldPolicy == null)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(hrPolicy);
                    await _context.SaveChangesAsync();
                }
            }


            var currentPolicy = await _context.NewHrPolicies.Where(a => a.PolicyName == hrPolicy.PolicyName).SingleOrDefaultAsync();
            addPolicy.PolicyID = currentPolicy.PolicyID;
            if (ModelState.IsValid)
            {
                _context.Add(addPolicy);
                await _context.SaveChangesAsync();
                return View();
            }
            return View();
        }
        public async Task<IActionResult> newPolicy(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AddNewPolicyVM policyVM = new AddNewPolicyVM();
            var Hrpolicy = await _context.NewHrPolicies.FirstOrDefaultAsync(m => m.PolicyID == id);
            var Addpolicy = await _context.AddPolicies.Where(m => m.PolicyID == id).ToListAsync();
            policyVM.NewHrPolicies = Hrpolicy;
            policyVM.newaddPolicies = Addpolicy;
            if (Hrpolicy == null)
            {
                return NotFound();
            }

            return View(policyVM);
        }

        [HttpPost]
        //public IActionResult ConvertHtmlToWord(string HtmlContentModel )
        //{
        //    if (HtmlContentModel == null || string.IsNullOrEmpty(HtmlContentModel))
        //    {
        //        return BadRequest("Invalid input.");
        //    }
        //    string htmlContent = @"
        //    <h1>HTML to Word Document</h1>
        //    <table border='1'>
        //        <tr>
        //            <th>Name</th>
        //            <th>Age</th>
        //        </tr>
        //        <tr>
        //            <td>John Doe</td>
        //            <td>30</td>
        //        </tr>
        //        <tr>
        //            <td>Jane Smith</td>
        //            <td>25</td>
        //        </tr>
        //    </table>
        //    <img src='~/images/sample.jpg' alt='Sample Image' />
        //";

        //}

        private async Task CreateCookies(Employee user)
        {
            var identity = new ClaimsIdentity("UserCookies", ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Role, "Employee"));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Emp_Name));
            identity.AddClaim(new Claim("Emp_Pic", user.Emp_Pic));
            identity.AddClaim(new Claim("Emp_ID", user.Emp_ID.ToString() ?? ""));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                "UserCookies",
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(2)
                });
        }


        //private IFormFile CreateFormFileFromFilePath(string filePath)
        //{
        //    var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        //    var formFile = CreateFormFileFromFilePath(path1);
        //    if (filePath == null)
        //    {
        //        return null;
        //    }

        //    var fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    var fileName = Path.GetFileName(filePath);
        //    var memoryStream = new MemoryStream(fileBytes);
        //    var formFile = new FormFile(memoryStream, 0, fileBytes.Length, null, fileName)
        //    {
        //        Headers = new HeaderDictionary(),
        //        ContentType = GetContentType(filePath)
        //    };

        //    return formFile;
        //}

        //private string GetContentType(string path)
        //{
        //    var types = GetMimeTypes();
        //    var ext = Path.GetExtension(path).ToLowerInvariant();
        //    return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        //}

        //private Dictionary<string, string> GetMimeTypes()
        //{
        //    return new Dictionary<string, string>
        //{
        //    {".txt", "text/plain"},
        //    {".pdf", "application/pdf"},
        //    {".doc", "application/vnd.ms-word"},
        //    {".docx", "application/vnd.ms-word"},
        //    {".xls", "application/vnd.ms-excel"},
        //    {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
        //    {".png", "image/png"},
        //    {".jpg", "image/jpeg"},
        //    {".jpeg", "image/jpeg"},
        //    {".gif", "image/gif"},
        //    {".csv", "text/csv"}
        //};
        //}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
