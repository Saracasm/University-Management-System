using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WP_project.Data;
using WP_project.Models;

namespace WP_project.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(UniversityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Student?> GetCurrentStudentAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.CourseOffering)
                .ThenInclude(co => co.Course)
                .FirstOrDefaultAsync(s => s.StudentId == user.StudentId);
        }

        public async Task<IActionResult> Index()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return RedirectToAction("Login", "Account");

            var myEnrollments = await _context.Enrollments
                .Include(e => e.CourseOffering)
                .ThenInclude(co => co.Course)
                .Include(e => e.CourseOffering.Semester)
                .Include(e => e.CourseOffering.Instructor)
                .Where(e => e.StudentId == student.StudentId)
                .OrderByDescending(e => e.CourseOffering.Semester.StartDate)
                .ToListAsync();

            return View(myEnrollments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return RedirectToAction("Login", "Account");

            var enrollment = await _context.Enrollments
                .Include(e => e.CourseOffering)
                .ThenInclude(c => c.Course)
                .Include(e => e.CourseOffering.Instructor)
                .FirstOrDefaultAsync(e => e.StudentId == student.StudentId && e.CourseOfferingId == id);

            if (enrollment == null) return NotFound();

            // Get Attendance Stats
            var attendanceRecords = await _context.Attendances
                .Where(a => a.StudentId == student.StudentId && a.CourseOfferingId == id)
                .OrderBy(a => a.Date)
                .ToListAsync();

            ViewBag.Attendance = attendanceRecords;
            ViewBag.TotalClasses = attendanceRecords.Count;
            ViewBag.PresentCount = attendanceRecords.Count(a => a.IsPresent);
            ViewBag.AttendancePercentage = attendanceRecords.Count > 0 
                ? (int)((double)ViewBag.PresentCount / ViewBag.TotalClasses * 100) 
                : 0;

            return View(enrollment);
        }

        public async Task<IActionResult> Register()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return RedirectToAction("Login", "Account");

            // Use UtcNow for PostgreSQL compatibility
            var activeSemesters = await _context.Semesters
                .Where(s => s.EndDate >= DateTime.UtcNow)
                .Select(s => s.SemesterId)
                .ToListAsync();

            var offerings = await _context.CourseOfferings
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .Where(c => activeSemesters.Contains(c.SemesterId))
                .ToListAsync();

            var enrolledOfferingIds = student.Enrollments.Select(e => e.CourseOfferingId).ToList();
            
            ViewBag.MyStudentId = student.StudentId;
            ViewBag.EnrolledIds = enrolledOfferingIds;

            return View(offerings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseOfferingId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return Unauthorized();

            var offering = await _context.CourseOfferings
                .Include(co => co.Course)
                .ThenInclude(c => c.Prerequisites)
                .FirstOrDefaultAsync(co => co.CourseOfferingId == courseOfferingId);

            if (offering == null) return NotFound();

            if (student.Enrollments.Any(e => e.CourseOfferingId == courseOfferingId))
            {
                TempData["Error"] = "You are already enrolled in this class.";
                return RedirectToAction(nameof(Register));
            }
            
            var completedCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == student.StudentId && e.Grade != null && e.Grade != "F")
                .Select(e => e.CourseOffering.CourseId)
                .ToListAsync();

            foreach (var prereq in offering.Course.Prerequisites)
            {
                if (!completedCourseIds.Contains(prereq.RequiredCourseId))
                {
                    TempData["Error"] = $"Prerequisite not met. You must complete Course ID {prereq.RequiredCourseId} first.";
                    return RedirectToAction(nameof(Register));
                }
            }

            var enrollment = new Enrollment
            {
                StudentId = student.StudentId,
                CourseOfferingId = courseOfferingId,
                EnrollmentDate = DateTime.UtcNow // Use UTC
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Successfully registered for {offering.Course.Code}!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Drop(int courseOfferingId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return Unauthorized();

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == student.StudentId && e.CourseOfferingId == courseOfferingId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course dropped successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
