using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WP_project.Data;
using WP_project.Models;

namespace WP_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly UniversityDbContext _context;

        public CoursesController(UniversityDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var universityDbContext = _context.Courses.Include(c => c.Department);
            return View(await universityDbContext.ToListAsync());
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Code,Title,Credits,DepartmentId")] Course course)
        {
            // Remove navigation property validation
            ModelState.Remove(nameof(course.Department));
            
            // Check if Code is unique
            if (await _context.Courses.AnyAsync(c => c.Code == course.Code))
            {
                 ModelState.AddModelError("Code", "Course Code already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // DEBUG: Log errors to console or show in view
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }
            
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", course.DepartmentId);
            return View(course);
        }

        // GET: Courses/Prerequisites/5
        public async Task<IActionResult> Prerequisites(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Prerequisites)
                .ThenInclude(p => p.RequiredCourse)
                .FirstOrDefaultAsync(m => m.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.AvailableCourses = new SelectList(
                _context.Courses.Where(c => c.CourseId != id && !c.Prerequisites.Any(p => p.RequiredCourseId == id)), // Prevent circular dependency simple check
                "CourseId", "Code");
            
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrerequisite(int CourseId, int RequiredCourseId)
        {
            if (CourseId == RequiredCourseId) return BadRequest("Cannot be prereq of itself");

            // Check duplicate
            if (!await _context.Prerequisites.AnyAsync(p => p.CourseId == CourseId && p.RequiredCourseId == RequiredCourseId))
            {
                var prereq = new Prerequisite { CourseId = CourseId, RequiredCourseId = RequiredCourseId };
                _context.Add(prereq);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Prerequisites), new { id = CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePrerequisite(int id)
        {
            var prereq = await _context.Prerequisites.FindAsync(id);
            if (prereq != null)
            {
                int courseId = prereq.CourseId;
                _context.Prerequisites.Remove(prereq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Prerequisites), new { id = courseId });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

