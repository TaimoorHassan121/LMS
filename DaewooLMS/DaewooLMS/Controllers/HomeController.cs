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
using DinkToPdf.Contracts;
using DinkToPdf;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "UserCookieScheme", Roles = "Employee")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        //private readonly IHTMLtoWord _htmltoword;
        private readonly IConverter _converter;

        public HomeController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RegisterModel> logger, IEmailSender emailSender,  IConverter converter)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
            //_htmltoword = hTMLtoWord;
            _converter = converter;
        }

        [BindProperty]
        public string ReturnUrl { get; set; }

        private List<AuthenticationScheme> ExternalLogins;

        public IActionResult Index()
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
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            HttpContext.Response.Cookies.Delete("UserCookies");
            HttpContext.Response.Cookies.Delete("UserCookies");
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
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
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
            long Emp_ID = HttpContext.User.Claims
             .Where(x => x.Type == "Emp_ID")
             .Select(x => Convert.ToInt64(x.Value))
             .FirstOrDefault();

            ProfileVM profileVM = new ProfileVM();

            var Profile = _context.Employees.Where(a=>a.Emp_ID == Emp_ID).Include(a=>a.Department).SingleOrDefault();
            var myNote = _context.MyNotes.Where(a=>a.Emp_Id == Emp_ID).ToList();

            profileVM.Employee = Profile;
            profileVM.myNotes = myNote;


            return View(profileVM);
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", employee.IdentityUserId);


            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Employee employee, IFormFile profilepic,int id)
        {      

            if (employee == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
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
                    else
                    {
                        var profilpic = _context.Employees.Where(a => a.Emp_ID == employee.Emp_ID).SingleOrDefault();
                        employee.Emp_Pic = profilpic.Emp_Pic;
                    }
                    employee.IsActive = true;
                    employee.Status = true;

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Profile));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }


            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", employee.IdentityUserId);


            return View();
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.UserId == id);
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


            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");

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
            chat.IsSeen = true;
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
            //chatReply.IsSeen = false;
            chatReply.IsSeen = true;
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
        public async Task<IActionResult> Events()
        {

            var events = await _context.Events.OrderByDescending(a=>a.EventID).ToListAsync();

            return View(events);
        }
        [HttpGet]
        public IActionResult Support()
        {
            var Emp_name = User.Claims.Where(a => a.Type == "EmpName")
                         .Select(x => x.Value)
                         .FirstOrDefault();

            return View();
        }
        public async Task<IActionResult> Support(long empId,string empName,string sub,string msg, Support support)
        {

            support.Emp_ID = empId;
            support.Emp_Name = empName;
            support.Subject = sub;
            support.Message = msg;  
            support.IsValid = true;
            support.MsgDate = DateTime.Now;

            _context.Add(support);
            await _context.SaveChangesAsync();
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
        public class HtmlContentModel
        {
            public string HtmlContent { get; set; }
        }


        public IActionResult ConvertHtmlToPdf(string HtmlContentModel)
        {

            var htmlContent = @"
        <html>
            <head>
                <style>
                    table, th, td {
                        border: 1px solid black;
                    }
                </style>
            </head>
            <body>
                <h1>Hello, World!</h1>
                <img src='https://via.placeholder.com/150' alt='Sample Image' />
                <table>
                    <tr>
                        <th>Header 1</th>
                        <th>Header 2</th>
                    </tr>
                    <tr>
                        <td>Data 1</td>
                        <td>Data 2</td>
                    </tr>
                </table>
            </body>
        </html>";
            if (HtmlContentModel == null || string.IsNullOrWhiteSpace(HtmlContentModel))
            {
                return BadRequest("Invalid HTML content");
            }

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
        },
                Objects = {
            new ObjectSettings() {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Footer" }
            }
        }
            };

            byte[] pdf = _converter.Convert(doc);

            return File(pdf, "application/pdf", "Invoice.pdf");
        }

    




        private async Task CreateCookies(Employee user)
        {
            var identity = new ClaimsIdentity("UserCookies", ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Role, "Employee"));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Emp_Name));
            identity.AddClaim(new Claim("EmpName", user.Emp_Name));
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

        public IActionResult Privacy()
        {
            return View();
        }

      
        public async Task<IActionResult> AddMyNotes(string msg, myNotes myNotes)
        {

            if(myNotes == null)
            {
                return View();
            }
            long Emp_ID = HttpContext.User.Claims
                .Where(x => x.Type == "Emp_ID")
                .Select(x => Convert.ToInt64(x.Value))
                .FirstOrDefault();
            myNotes.myNote = msg;
            myNotes.status= true;
            myNotes.Date = DateTime.Now;
            myNotes.Emp_Id = Emp_ID;

            _context.Add(myNotes);
            await _context.SaveChangesAsync();
            return View();
        }

        public async Task<IActionResult> DeleteMyNote(int id)
        {

            if(id == 0)
            {
                return View();
            }

            var myNoteDb = await _context.MyNotes.FindAsync(id);
            _context.MyNotes.Remove(myNoteDb);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> QuizAttemp(QuizAttempt quizAttempt,string Department,string score,string status)
        {

            long Emp_ID = User.Claims
                .Where(x => x.Type == "Emp_ID")
                .Select(x => Convert.ToInt64(x.Value))
                .FirstOrDefault();


            var quizAtmp = _context.QuizAttempts.Where(a => a.Emp_ID == Emp_ID).OrderByDescending(a => a.QuizAtmp_ID).FirstOrDefault();
         
            if (quizAtmp != null)
            {
                if (quizAtmp.Score == "0" && status == "QuizStart")
                {
                    int attemp;
                    attemp = quizAtmp.Quiz_Attempts + 1;
                    quizAtmp.Quiz_Attempts = attemp;
                    quizAtmp.Score = score;
                    quizAtmp.StartDate = DateTime.Now;

                    _context.QuizAttempts.Update(quizAtmp);
                    await _context.SaveChangesAsync();
                    return View();

                }
                if(status == "Submit")
                {
                    quizAtmp.Score = score;
                    quizAtmp.EndDate = DateTime.Now;
                    quizAtmp.Status = status;

                    _context.QuizAttempts.Update(quizAtmp);
                    await _context.SaveChangesAsync();
                    return View();
                }

            }

            quizAttempt.Emp_ID = Emp_ID;
            quizAttempt.Quiz_Attempts = 1;
            quizAttempt.Score = score;
            quizAttempt.Emp_Department = Department;
            quizAttempt.StartDate = DateTime.Now;
            quizAttempt.EndDate = null;
            quizAttempt.Status = status;

            _context.QuizAttempts.Add(quizAttempt);
            await _context.SaveChangesAsync();
            return View();
        }








            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
