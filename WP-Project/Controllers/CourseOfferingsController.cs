using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WP_project.Data;
using WP_project.Models;

namespace WP_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseOfferingsController : Controller
    {
        private readonly UniversityDbContext _context;

        public CourseOfferingsController(UniversityDbContext context)
        {
            _context = context;
        }

        // GET: CourseOfferings
        public async Task<IActionResult> Index()
        {
            var offerings = _context.CourseOfferings
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester);
            return View(await offerings.ToListAsync());
        }

        // GET: CourseOfferings/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Code");
            ViewData["InstructorId"] = new SelectList(_context.Faculties, "FacultyId", "Name");
            // Use UtcNow to match PostgreSQL timestamp with time zone
            ViewData["SemesterId"] = new SelectList(_context.Semesters.Where(s => s.EndDate > DateTime.UtcNow), "SemesterId", "Name"); 
            return View();
        }

        // POST: CourseOfferings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseOfferingId,CourseId,SemesterId,InstructorId,Section,Capacity")] CourseOffering courseOffering)
        {
            // Fix validation for Navigation Properties
            ModelState.Remove(nameof(courseOffering.Course));
            ModelState.Remove(nameof(courseOffering.Semester));
            ModelState.Remove(nameof(courseOffering.Instructor));

            if (ModelState.IsValid)
            {
                // Optional: Check if instructor is already teaching this course in this semester (Section logic)
                _context.Add(courseOffering);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Debug errors if any
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach(var e in errors) System.Diagnostics.Debug.WriteLine(e.ErrorMessage);

            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Code", courseOffering.CourseId);
            ViewData["InstructorId"] = new SelectList(_context.Faculties, "FacultyId", "Name", courseOffering.InstructorId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "SemesterId", "Name", courseOffering.SemesterId);
            return View(courseOffering);
        }
        
         // GET: CourseOfferings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseOffering = await _context.CourseOfferings
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .FirstOrDefaultAsync(m => m.CourseOfferingId == id);
            if (courseOffering == null)
            {
                return NotFound();
            }

            return View(courseOffering);
        }

        // POST: CourseOfferings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseOffering = await _context.CourseOfferings.FindAsync(id);
            if (courseOffering != null)
            {
                _context.CourseOfferings.Remove(courseOffering);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

