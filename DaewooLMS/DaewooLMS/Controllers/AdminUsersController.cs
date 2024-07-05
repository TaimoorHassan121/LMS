using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DaewooLMS.Data;
using DaewooLMS.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using DaewooLMS.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using DaewooLMS.Models.ViewModel;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "AdminCookieScheme", Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public AdminUsersController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RegisterModel> logger, IEmailSender emailSender)
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

        // GET: AdminUsers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AdminUsers.Include(a => a.IdentityUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AdminUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers
                .Include(a => a.IdentityUser)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        // GET: AdminUsers/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["IdentityRoleId"] = new SelectList(_roleManager.Roles, "Id", "Name");
            return View();
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,IdentityUserId,User_EmpId,User_Name,User_Designation,User_Department,Role,Mobile_No,User_Email,User_Passward,User_Status,User_Date,ProfilePhoto")] AdminUser adminUser, IFormFile profilepic, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var AdminUser = _context.AdminUsers.Where(a => a.User_EmpId == adminUser.User_EmpId).SingleOrDefault();
            var roles = await _roleManager.Roles.Where(a => a.Id == adminUser.Role).SingleOrDefaultAsync();
            adminUser.Role = roles.ToString();
            //adminUser.IdentityRole = roles;
            if (AdminUser != null)
            {
                return View(adminUser);
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = adminUser.User_Email, Email = adminUser.User_Email, PhoneNumber = adminUser.Mobile_No, EmailConfirmed = true };
                adminUser.IdentityUser = user;
                if (profilepic != null)
                {
                    var image = ContentDispositionHeaderValue.Parse(profilepic.ContentDisposition).FileName.Trim();
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", profilepic.FileName);
                    using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                    {
                        profilepic.CopyTo(stream);
                    }
                    adminUser.ProfilePhoto = profilepic.FileName;
                }
                var result = await _userManager.CreateAsync(user, adminUser.User_Passward);
                if (result.Succeeded)
                {
                    //await _roleManager.CreateAsync(new IdentityRole(adminUser.Role));
                    await _userManager.AddToRoleAsync(user, adminUser.Role);

                    _logger.LogInformation("User created a new account with password.");
                    _context.Add(adminUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", adminUser.IdentityUserId);
            ViewData["IdentityRoleId"] = new SelectList(_roleManager.Roles, "Id", "Name");
            return View(adminUser);
        }

        // GET: AdminUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers.FindAsync(id);
            if (adminUser == null)
            {
                return NotFound();
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", adminUser.IdentityUserId);
            ViewData["IdentityRoleId"] = new SelectList(_roleManager.Roles, "Id", "Name");
            return View(adminUser);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,IdentityUserId,User_EmpId,User_Name,User_Designation,User_Department,Role,Mobile_No,User_Email,User_Passward,User_Status,User_Date,ProfilePhoto")] AdminUser adminUser, IFormFile profilepic, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var roles = await _roleManager.Roles.Where(a => a.Id == adminUser.Role).SingleOrDefaultAsync();
            adminUser.Role = roles.ToString();

            if (id != adminUser.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.AdminUsers.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    if (profilepic != null)
                    {
                        var image = ContentDispositionHeaderValue.Parse(profilepic.ContentDisposition).FileName.Trim();
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", profilepic.FileName);
                        using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                        {
                            profilepic.CopyTo(stream);
                        }
                        adminUser.ProfilePhoto = profilepic.FileName;
                    }
                    else
                    {
                        var profilpic = _context.AdminUsers.Where(a => a.User_EmpId == adminUser.User_EmpId).SingleOrDefault();
                        adminUser.ProfilePhoto = profilpic.ProfilePhoto;
                    }

                    existingUser.IdentityUserId = adminUser.IdentityUserId;
                    existingUser.User_EmpId = adminUser.User_EmpId;
                    existingUser.User_Name = adminUser.User_Name;
                    existingUser.User_Designation = adminUser.User_Designation;
                    existingUser.User_Department = adminUser.User_Department;
                    existingUser.Role = adminUser.Role;
                    existingUser.Mobile_No = adminUser.Mobile_No;
                    existingUser.User_Email = adminUser.User_Email;
                    existingUser.User_Passward = adminUser.User_Passward;
                    existingUser.User_Status = adminUser.User_Status;
                    existingUser.User_Date = adminUser.User_Date;
                    existingUser.ProfilePhoto = adminUser.ProfilePhoto;

                    _logger.LogInformation("User edited their account.");
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminUserExists(adminUser.UserId))
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
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", adminUser.IdentityUserId);
            ViewData["IdentityRoleId"] = new SelectList(_roleManager.Roles, "Id", "Name");
            return View(adminUser);
        }

        // GET: AdminUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers
                .Include(a => a.IdentityUser)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminUser = await _context.AdminUsers.FindAsync(id);
            _context.AdminUsers.Remove(adminUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminUserExists(int id)
        {
            return _context.AdminUsers.Any(e => e.UserId == id);
        }

        public async Task<IActionResult> ChangeStatusAdminUser(int id)
        {
            var AdminDB = await _context.AdminUsers.FindAsync(id);
            if (AdminDB != null)
            {
                AdminDB.User_Status = !AdminDB.User_Status;
                _context.AdminUsers.Update(AdminDB);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AdminIndex()
        {

            AdminIndexVM adminVM = new AdminIndexVM();

            var emp = _context.Employees.Include(a => a.Department).ToList();
            var chatM = _context.Emp_Chat_M.ToList();
            var chatReply = _context.Emp_Chat_Replies.ToList();
            var support = _context.Support.ToList();
            var eventDB = _context.Events.ToList();


            adminVM.employees = emp.OrderByDescending(a => a.UserId).Take(5).ToList();
            adminVM.chatMaster = chatM.OrderByDescending(a => a.ChatID).Take(5).ToList();
            adminVM.chatReply = chatReply.OrderByDescending(a => a.Chat_ReplyID).Take(5).ToList();
            adminVM.support = support.OrderByDescending(a => a.SupportID).Take(5).ToList();
            adminVM.events = eventDB.OrderByDescending(a => a.EventID).Take(5).ToList();


            return View(adminVM);
        }
    }
}
