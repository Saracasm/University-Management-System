using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WP_project.Data;
using WP_project.Models;

namespace WP_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SemestersController : Controller
    {
        private readonly UniversityDbContext _context;

        public SemestersController(UniversityDbContext context)
        {
            _context = context;
        }

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Semesters.OrderByDescending(s => s.StartDate).ToListAsync());
        }

        // GET: Semesters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Semesters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SemesterId,Name,StartDate,EndDate")] Semester semester)
        {
            if (ModelState.IsValid)
            {
                // Fix for PostgreSQL: Ensure dates are UTC
                semester.StartDate = DateTime.SpecifyKind(semester.StartDate, DateTimeKind.Utc);
                semester.EndDate = DateTime.SpecifyKind(semester.EndDate, DateTimeKind.Utc);

                _context.Add(semester);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(semester);
        }
        
        // GET: Semesters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters.FindAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        // POST: Semesters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SemesterId,Name,StartDate,EndDate")] Semester semester)
        {
            if (id != semester.SemesterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fix for PostgreSQL: Ensure dates are UTC
                    semester.StartDate = DateTime.SpecifyKind(semester.StartDate, DateTimeKind.Utc);
                    semester.EndDate = DateTime.SpecifyKind(semester.EndDate, DateTimeKind.Utc);

                    _context.Update(semester);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Semesters.Any(e => e.SemesterId == id))
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
            return View(semester);
        }
    }
}

