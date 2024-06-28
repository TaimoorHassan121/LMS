using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DaewooLMS.Data;
using DaewooLMS.Models;

namespace DaewooLMS.Controllers
{
    public class QuizQuestionAnswersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizQuestionAnswersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: QuizQuestionAnswers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.QuizQuestionAnswer.Include(q => q.Departments);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: QuizQuestionAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quizQuestionAnswer = await _context.QuizQuestionAnswer
                .Include(q => q.Departments)
                .FirstOrDefaultAsync(m => m.QuizID == id);
            if (quizQuestionAnswer == null)
            {
                return NotFound();
            }

            return View(quizQuestionAnswer);
        }

        // GET: QuizQuestionAnswers/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: QuizQuestionAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuizID,QuizQno,Question,Answer,DepartmentID,QStatus,QuizDate")] QuizQuestionAnswer quizQuestionAnswer, QuizLog quizLog)
        {
            var Department = quizQuestionAnswer.DepartmentID;
            var quizNo = quizQuestionAnswer.QuizQno;
            var Question = quizQuestionAnswer.Question;
            var Answer = quizQuestionAnswer.Answer;
            var status = quizQuestionAnswer.QStatus;
            var QuizDate = quizQuestionAnswer.QuizDate;
            var depat = _context.Departments.Where(a => a.DepartmentID == Department).SingleOrDefault();
            var option = _context.QuizOptions.Where(a => a.QuizNo == quizNo && a.Department == Department).ToList();
            foreach (var item in option)
            {
                if (item.OptionNo == 1) { quizLog.option1 = item.Options; }
                if (item.OptionNo == 2) { quizLog.option2 = item.Options; }
                if (item.OptionNo == 3) { quizLog.option3 = item.Options; }
                if (item.OptionNo == 4) { quizLog.option4 = item.Options; }
                if (item.OptionNo == 5) { quizLog.option5 = item.Options; }
            }
            quizLog.Department = depat.DepartmentName;
            quizLog.QuestionId = quizNo;
            quizLog.Answer = Answer;
            quizLog.QuizStatus = status;
            quizLog.Quiz_AddDate = QuizDate;
            if (ModelState.IsValid)
            {
                _context.Add(quizQuestionAnswer);
                _context.Add(quizLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", quizQuestionAnswer.DepartmentID);
            return View(quizQuestionAnswer);
        }

        // GET: QuizQuestionAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quizQuestionAnswer = await _context.QuizQuestionAnswer.FindAsync(id);
            if (quizQuestionAnswer == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", quizQuestionAnswer.DepartmentID);
            return View(quizQuestionAnswer);
        }

        // POST: QuizQuestionAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuizID,QuizQno,Question,Answer,DepartmentID,QStatus,QuizDate")] QuizQuestionAnswer quizQuestionAnswer, QuizLog quizLog)
        {
            if (id != quizQuestionAnswer.QuizID)
            {
                return NotFound();
            }

            var Department = quizQuestionAnswer.DepartmentID;
            var quizNo = quizQuestionAnswer.QuizQno;
            var Question = quizQuestionAnswer.Question;
            var Answer = quizQuestionAnswer.Answer;
            var status = quizQuestionAnswer.QStatus;
            var QuizDate = quizQuestionAnswer.QuizDate;
            //var depat = _context.Departments.Where(a => a.Department_ID == Department).SingleOrDefault();
            var option = _context.QuizOptions.Where(a => a.QuizNo == quizNo && a.Department == Department).ToList();
            foreach (var item in option)
            {
                if (item.OptionNo == 1) { quizLog.option1 = item.Options; }
                if (item.OptionNo == 2) { quizLog.option2 = item.Options; }
                if (item.OptionNo == 3) { quizLog.option3 = item.Options; }
                if (item.OptionNo == 4) { quizLog.option4 = item.Options; }
                if (item.OptionNo == 5) { quizLog.option5 = item.Options; }
            }
            //quizLog.Department = depat.Department_Name;
            quizLog.QuizLogId = quizNo;
            quizLog.Answer = Answer;
            quizLog.QuizStatus = status;
            quizLog.Quiz_UpdateDate = QuizDate;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quizQuestionAnswer);
                    _context.Add(quizLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizQuestionAnswerExists(quizQuestionAnswer.QuizID))
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
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", quizQuestionAnswer.DepartmentID);
            return View(quizQuestionAnswer);
        }

        // GET: QuizQuestionAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quizQuestionAnswer = await _context.QuizQuestionAnswer
                .Include(q => q.Departments)
                .FirstOrDefaultAsync(m => m.QuizID == id);
            if (quizQuestionAnswer == null)
            {
                return NotFound();
            }

            return View(quizQuestionAnswer);
        }

        // POST: QuizQuestionAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quizQuestionAnswer = await _context.QuizQuestionAnswer.FindAsync(id);
            _context.QuizQuestionAnswer.Remove(quizQuestionAnswer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizQuestionAnswerExists(int id)
        {
            return _context.QuizQuestionAnswer.Any(e => e.QuizID == id);
        }
    }
}
