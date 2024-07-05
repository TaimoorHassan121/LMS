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
    public class Emp_Chat_ReplyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Emp_Chat_ReplyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Emp_Chat_Reply
        public async Task<IActionResult> Index()
        {
            return View(await _context.Emp_Chat_Replies.ToListAsync());
        }

        // GET: Emp_Chat_Reply/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp_Chat_Reply = await _context.Emp_Chat_Replies
                .FirstOrDefaultAsync(m => m.Chat_ReplyID == id);
            if (emp_Chat_Reply == null)
            {
                return NotFound();
            }

            return View(emp_Chat_Reply);
        }

        // GET: Emp_Chat_Reply/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Emp_Chat_Reply/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Chat_ReplyID,ChatM_ID,Emp_ChatM_ID,Emp_Reply_ID,Reply_Message,Emp_Name,Emp_Pic_Reply,IsSeen,Status,MsgDate")] Emp_Chat_Reply emp_Chat_Reply)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emp_Chat_Reply);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emp_Chat_Reply);
        }

        // GET: Emp_Chat_Reply/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp_Chat_Reply = await _context.Emp_Chat_Replies.FindAsync(id);
            if (emp_Chat_Reply == null)
            {
                return NotFound();
            }
            return View(emp_Chat_Reply);
        }

        // POST: Emp_Chat_Reply/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Chat_ReplyID,ChatM_ID,Emp_ChatM_ID,Emp_Reply_ID,Reply_Message,Emp_Name,Emp_Pic_Reply,IsSeen,Status,MsgDate")] Emp_Chat_Reply emp_Chat_Reply)
        {
            if (id != emp_Chat_Reply.Chat_ReplyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emp_Chat_Reply);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Emp_Chat_ReplyExists(emp_Chat_Reply.Chat_ReplyID))
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
            return View(emp_Chat_Reply);
        }

        // GET: Emp_Chat_Reply/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emp_Chat_Reply = await _context.Emp_Chat_Replies
                .FirstOrDefaultAsync(m => m.Chat_ReplyID == id);
            if (emp_Chat_Reply == null)
            {
                return NotFound();
            }

            return View(emp_Chat_Reply);
        }

        // POST: Emp_Chat_Reply/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp_Chat_Reply = await _context.Emp_Chat_Replies.FindAsync(id);
            _context.Emp_Chat_Replies.Remove(emp_Chat_Reply);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Emp_Chat_ReplyExists(int id)
        {
            return _context.Emp_Chat_Replies.Any(e => e.Chat_ReplyID == id);
        }

        public async Task<IActionResult> ChangeChatReplyStatus(int id)
        {
            var ChatReplyDB = await _context.Emp_Chat_Replies.FindAsync(id);
            if (ChatReplyDB != null)
            {
                ChatReplyDB.IsSeen = !ChatReplyDB.IsSeen;
                _context.Emp_Chat_Replies.Update(ChatReplyDB);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
