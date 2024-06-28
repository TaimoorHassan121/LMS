using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DaewooLMS.Data;
using DaewooLMS.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace DaewooLMS.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,Event_Title,Objactive,Participent,Event_PIC,start_DateTime,End_DateTime,Add_DateTime,status")] Event @event, IFormFile eventImg, Event_Logs logEvent)
        {
            if (ModelState.IsValid)
            {
                long AdminEmp_ID = HttpContext.User.Claims
                  .Where(x => x.Type == "User_EmpId")
                  .Select(x => Convert.ToInt64(x.Value))
                  .FirstOrDefault();
                if (eventImg != null)
                {
                    var image = ContentDispositionHeaderValue.Parse(eventImg.ContentDisposition).FileName.Trim();
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventImages", eventImg.FileName);
                    using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                    {
                        eventImg.CopyTo(stream);
                    }
                    @event.Event_PIC = eventImg.FileName;
                }

                logEvent.EventID = @event.EventID;
                logEvent.Event_PIC= @event.Event_PIC;
                logEvent.Event_Title = @event.Event_Title;  
                logEvent.Objactive = @event.Objactive;
                logEvent.Participent = @event.Participent;
                logEvent.start_DateTime = @event.start_DateTime;
                logEvent.End_DateTime = @event.End_DateTime;
                logEvent.status = @event.status;
                logEvent.Authorize_Person = AdminEmp_ID;
                logEvent.CRUD_Status = "Create";
                logEvent.Add_Update_DateTime = @event.Add_DateTime;
                _context.Add(@event);
                _context.Add(logEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,Event_Title,Objactive,Participent,Event_PIC,start_DateTime,End_DateTime,Add_DateTime,status")] Event @event, Event_Logs eventLog, IFormFile EventImg)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (EventImg != null)
                    {
                        var image = ContentDispositionHeaderValue.Parse(EventImg.ContentDisposition).FileName.Trim();
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventImages", EventImg.FileName);
                        using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                        {
                            EventImg.CopyTo(stream);
                        }
                        @event.Event_PIC = EventImg.FileName;
                    }
                    else
                    {
                        var EventPic = _context.Events.Where(a => a.EventID == @event.EventID).SingleOrDefault();
                        @event.Event_PIC = EventPic.Event_PIC;

                    }
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}
