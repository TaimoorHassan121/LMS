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
    public class LibraryDatasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LibraryDatas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LibraryDatas.Include(l => l.Departments);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: LibraryDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryData = await _context.LibraryDatas
                .Include(l => l.Departments)
                .FirstOrDefaultAsync(m => m.PDFID == id);
            if (libraryData == null)
            {
                return NotFound();
            }

            return View(libraryData);
        }

        // GET: LibraryDatas/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: LibraryDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PDFID,Pdf_Name,Pdf_File,Trade,DepartmentID,IsActive,Status,Date")] LibraryData libraryData, IFormFile PdfFile)
        {
            if (ModelState.IsValid)
            {
                if (PdfFile != null)
                {
                    var image = ContentDispositionHeaderValue.Parse(PdfFile.ContentDisposition).FileName.Trim();
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CoursePDF", PdfFile.FileName);
                    using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                    {
                        PdfFile.CopyTo(stream);
                    }
                    libraryData.Pdf_File = PdfFile.FileName;
                }
                _context.Add(libraryData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", libraryData.DepartmentID);
            return View(libraryData);
        }

        // GET: LibraryDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryData = await _context.LibraryDatas.FindAsync(id);
            if (libraryData == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", libraryData.DepartmentID);
            return View(libraryData);
        }

        // POST: LibraryDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PDFID,Pdf_Name,Pdf_File,Trade,DepartmentID,IsActive,Status,Date")] LibraryData libraryData, IFormFile PdfFile)
        {
            if (id != libraryData.PDFID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (PdfFile != null)
                    {
                        var image = ContentDispositionHeaderValue.Parse(PdfFile.ContentDisposition).FileName.Trim();
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CoursePDF", PdfFile.FileName);
                        using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                        {
                            PdfFile.CopyTo(stream);
                        }
                        libraryData.Pdf_File = PdfFile.FileName;
                    }
                    else
                    {
                        var PDFFile = _context.LibraryDatas.Where(a => a.PDFID == libraryData.PDFID).SingleOrDefault();
                        libraryData.Pdf_File = PDFFile.Pdf_File;
                    }
                    _context.Update(libraryData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryDataExists(libraryData.PDFID))
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
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", libraryData.DepartmentID);
            return View(libraryData);
        }

        // GET: LibraryDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryData = await _context.LibraryDatas
                .Include(l => l.Departments)
                .FirstOrDefaultAsync(m => m.PDFID == id);
            if (libraryData == null)
            {
                return NotFound();
            }

            return View(libraryData);
        }

        // POST: LibraryDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libraryData = await _context.LibraryDatas.FindAsync(id);
            _context.LibraryDatas.Remove(libraryData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryDataExists(int id)
        {
            return _context.LibraryDatas.Any(e => e.PDFID == id);
        }
        public async Task<IActionResult> ChangeStatusPDF(int id)
        {
            var PDFData = await _context.LibraryDatas.FindAsync(id);
            if (PDFData != null)
            {
                PDFData.IsActive = !PDFData.IsActive;
                _context.LibraryDatas.Update(PDFData);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
