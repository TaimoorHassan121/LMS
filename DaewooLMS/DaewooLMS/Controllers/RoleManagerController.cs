using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "AdminCookieScheme", Roles = "Admin")]
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["Message"] = "Role name cannot be empty";
                return RedirectToAction("Index");
            }

            var existingRole = await _roleManager.FindByNameAsync(roleName.Trim());

            if (existingRole != null)
            {
                TempData["Message"] = "Role already exists";
                return RedirectToAction("Index");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

            if (result.Succeeded)
            {
                TempData["Message"] = "Role created successfully";
            }
            else
            {
                TempData["Message"] = "Error creating role";
            }

            return RedirectToAction("Index");
        }
    }
}
