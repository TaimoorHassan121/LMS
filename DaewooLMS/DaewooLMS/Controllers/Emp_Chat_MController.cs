using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DaewooLMS.Data;
using DaewooLMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "AdminCookieScheme", Roles = "Admin")]
    public class Emp_Chat_MController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Emp_Chat_MController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Emp_Chat_M
        public async Task<IActionResult> Index()
        {
            return View(await _context.Emp_Chat_M.ToListAsync());
        }

        // GET: Emp_Chat_M/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp_Chat_M = await _context.Emp_Chat_M
                .FirstOrDefaultAsync(m => m.ChatID == id);
            if (emp_Chat_M == null)
            {
                return NotFound();
            }

            return View(emp_Chat_M);
        }

        // GET: Emp_Chat_M/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Emp_Chat_M/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChatID,Emp_ID,Emp_Name,Emp_Designation,message,Emp_Pic,IsSeen,Status,MsgDate")] Emp_Chat_M emp_Chat_M)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emp_Chat_M);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emp_Chat_M);
        }

        // GET: Emp_Chat_M/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp_Chat_M = await _context.Emp_Chat_M.FindAsync(id);
            if (emp_Chat_M == null)
            {
                return NotFound();
            }
            return View(emp_Chat_M);
        }

        // POST: Emp_Chat_M/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChatID,Emp_ID,Emp_Name,Emp_Designation,message,Emp_Pic,IsSeen,Status,MsgDate")] Emp_Chat_M emp_Chat_M)
        {
            if (id != emp_Chat_M.ChatID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emp_Chat_M);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Emp_Chat_MExists(emp_Chat_M.ChatID))
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
            return View(emp_Chat_M);
        }

        // GET: Emp_Chat_M/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp_Chat_M = await _context.Emp_Chat_M
                .FirstOrDefaultAsync(m => m.ChatID == id);
            if (emp_Chat_M == null)
            {
                return NotFound();
            }

            return View(emp_Chat_M);
        }

        // POST: Emp_Chat_M/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp_Chat_M = await _context.Emp_Chat_M.FindAsync(id);
            _context.Emp_Chat_M.Remove(emp_Chat_M);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Emp_Chat_MExists(int id)
        {
            return _context.Emp_Chat_M.Any(e => e.ChatID == id);
        }

        public async Task<IActionResult> ChangeChatStatusM(int id)
        {
            var ChatM = await _context.Emp_Chat_M.FindAsync(id);
            if (ChatM != null)
            {
                ChatM.IsSeen = !ChatM.IsSeen;
                _context.Emp_Chat_M.Update(ChatM);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
