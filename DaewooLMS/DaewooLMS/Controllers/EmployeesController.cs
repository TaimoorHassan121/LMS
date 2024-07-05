using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DaewooLMS.Data;
using DaewooLMS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using DaewooLMS.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "AdminCookieScheme", Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;


        public EmployeesController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RegisterModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        [BindProperty]
        public string ReturnUrl { get; set; }

        private List<AuthenticationScheme> ExternalLogins;

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.Department).Include(e => e.IdentityUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.IdentityUser)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,IdentityUserId,Emp_ID,Emp_Name,Emp_Pic,Designation,DepartmentID,Mobile_No,User_Passward,Gender,IsActive,Status,Emp_Date")] Employee employee, IFormFile profilepic, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var useremployee = _context.Employees.Where(a => a.Emp_ID == employee.Emp_ID).SingleOrDefault();
            //var roles = await _roleManager.Roles.Where(a => a.Id == employee.Role).SingleOrDefaultAsync();

            if (useremployee != null)
            {
                TempData["message"] = "Employee Already Available";
                return View(employee);
            }

            if (ModelState.IsValid)
            {
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
                    return RedirectToAction(nameof(Index));

                }
                //_context.Add(employee);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", employee.IdentityUserId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,IdentityUserId,Emp_ID,Emp_Name,Emp_Pic,Designation,DepartmentID,Mobile_No,User_Passward,Gender,IsActive,Status,Emp_Date")] Employee employee, IFormFile profilepic, string returnUrl = null)
        {
            if (id != employee.UserId)
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

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.IdentityUser)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.UserId == id);
        }

        public async Task<IActionResult> ChangeStatusEmp(int id)
        {
            var EmpDB = await _context.Employees.FindAsync(id);
            if (EmpDB != null)
            {
                EmpDB.IsActive = !EmpDB.IsActive;
                _context.Employees.Update(EmpDB);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
