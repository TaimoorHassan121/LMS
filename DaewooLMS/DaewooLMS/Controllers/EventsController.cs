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
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "AdminCookieScheme", Roles = "Admin")]
    //[Authorize(Roles = "Admin")]
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

            var events = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (events == null)
            {
                return NotFound();
            }

            return View(events);
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
        public async Task<IActionResult> Create([Bind("EventID,Event_Title,Objactive,Participent,Event_PIC,start_DateTime,End_DateTime,Add_DateTime,status")] Events events,Event_Logs event_Logs,IFormFile eventPic)
        {
            if (ModelState.IsValid)
            {

                var AdminId = HttpContext.User.Claims
                  .Where(x => x.Type == "UserID")
                  .Select(x => Convert.ToInt64(x.Value))
                  .FirstOrDefault();



                if (eventPic != null)
                {
                    var image = ContentDispositionHeaderValue.Parse(eventPic.ContentDisposition).FileName.Trim();
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventImages", eventPic.FileName);
                    using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                    {
                        eventPic.CopyTo(stream);
                    }
                    events.Event_PIC = eventPic.FileName;
                }

                _context.Add(events);
                await _context.SaveChangesAsync();


                event_Logs.EventID = events.EventID;
                event_Logs.Event_Title = events.Event_Title;
                event_Logs.Objactive = events.Objactive;
                event_Logs.Event_PIC = events.Event_PIC;
                event_Logs.CRUD_Status = "Create";
                event_Logs.Add_Update_DateTime = events.Add_DateTime;
                event_Logs.start_DateTime = events.start_DateTime;
                event_Logs.End_DateTime = events.End_DateTime;
                event_Logs.Authorize_Person = AdminId;
                event_Logs.status = events.status;

                _context.Add(event_Logs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(events);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            return View(events);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,Event_Title,Objactive,Participent,Event_PIC,start_DateTime,End_DateTime,Add_DateTime,status")] Events events, Event_Logs event_Logs, IFormFile eventPic)
        {
            if (id != events.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var AdminId = HttpContext.User.Claims
                      .Where(x => x.Type == "UserID")
                      .Select(x => Convert.ToInt64(x.Value))
                      .FirstOrDefault();
                    if (eventPic != null)
                    {
                        var image = ContentDispositionHeaderValue.Parse(eventPic.ContentDisposition).FileName.Trim();
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventImages", eventPic.FileName);
                        using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                        {
                            eventPic.CopyTo(stream);
                        }
                        events.Event_PIC = eventPic.FileName;
                    }
                    else
                    {
                        var EventPic = _context.Events.Where(a => a.EventID == events.EventID).SingleOrDefault();
                        events.Event_PIC = EventPic.Event_PIC;

                    }
                   

                    event_Logs.EventID = events.EventID;
                    event_Logs.Event_Title = events.Event_Title;
                    event_Logs.Objactive = events.Objactive;
                    event_Logs.Event_PIC = events.Event_PIC;
                    event_Logs.CRUD_Status = "Update";
                    event_Logs.Add_Update_DateTime = events.Add_DateTime;
                    event_Logs.start_DateTime = events.start_DateTime;
                    event_Logs.End_DateTime = events.End_DateTime;
                    event_Logs.Authorize_Person = AdminId;
                    event_Logs.status = events.status;

                    _context.Update(events);                  
                    _context.Add(event_Logs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsExists(events.EventID))
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
            return View(events);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var events = await _context.Events.FindAsync(id);
            _context.Events.Remove(events);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }

        public async Task<IActionResult> ChangeStatusEvent(int id)
        {
            var EventDb = await _context.Events.FindAsync(id);
            if (EventDb != null)
            {
                EventDb.status = !EventDb.status;
                _context.Events.Update(EventDb);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
