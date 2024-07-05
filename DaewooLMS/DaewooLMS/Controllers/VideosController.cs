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
using Microsoft.AspNetCore.Authorization;

namespace DaewooLMS.Controllers
{
    [Authorize(Policy = "AdminCookieScheme", Roles = "Admin")]
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VideosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Videos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Videos.Include(v => v.Departments);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Videos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videos = await _context.Videos
                .Include(v => v.Departments)
                .FirstOrDefaultAsync(m => m.VideoID == id);
            if (videos == null)
            {
                return NotFound();
            }

            return View(videos);
        }

        // GET: Videos/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: Videos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VideoID,Video_Name,Video_Link,Trade,DepartmentID,IsActive,Status,Date")] Videos videos, IFormFile VideoFile)
        {
            if (ModelState.IsValid)
            {
                if (VideoFile != null)
                {
                    var image = ContentDispositionHeaderValue.Parse(VideoFile.ContentDisposition).FileName.Trim();
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", VideoFile.FileName);
                    using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                    {
                        VideoFile.CopyTo(stream);
                    }
                    videos.Video_Link = VideoFile.FileName;
                }
                _context.Add(videos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", videos.DepartmentID);
            return View(videos);
        }

        // GET: Videos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videos = await _context.Videos.FindAsync(id);
            if (videos == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", videos.DepartmentID);
            return View(videos);
        }

        // POST: Videos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VideoID,Video_Name,Video_Link,Trade,DepartmentID,IsActive,Status,Date")] Videos videos, IFormFile VideoFile)
        {
            if (id != videos.VideoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (VideoFile != null)
                    {
                        var image = ContentDispositionHeaderValue.Parse(VideoFile.ContentDisposition).FileName.Trim();
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", VideoFile.FileName);
                        using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                        {
                            VideoFile.CopyTo(stream);
                        }
                        videos.Video_Link = VideoFile.FileName;
                    }
                    else
                    {
                        var VideoLink = _context.Videos.Where(a => a.VideoID == videos.VideoID).SingleOrDefault();
                        videos.Video_Link = VideoLink.Video_Link;
                    }
                    _context.Update(videos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideosExists(videos.VideoID))
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
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", videos.DepartmentID);
            return View(videos);
        }

        // GET: Videos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videos = await _context.Videos
                .Include(v => v.Departments)
                .FirstOrDefaultAsync(m => m.VideoID == id);
            if (videos == null)
            {
                return NotFound();
            }

            return View(videos);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var videos = await _context.Videos.FindAsync(id);
            _context.Videos.Remove(videos);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideosExists(int id)
        {
            return _context.Videos.Any(e => e.VideoID == id);
        }

        public async Task<IActionResult> ChangeStatusVideo(int id)
        {
            var videoData = await _context.Videos.FindAsync(id);
            if (videoData != null)
            {
                videoData.IsActive = !videoData.IsActive;
                _context.Videos.Update(videoData);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
