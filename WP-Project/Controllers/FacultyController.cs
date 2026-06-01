using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WP_project.Data;
using WP_project.Models;

namespace WP_project.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FacultyController(UniversityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Faculty?> GetCurrentFacultyAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            return await _context.Faculties
                .FirstOrDefaultAsync(f => f.FacultyId == user.FacultyId);
        }

        // 1. My Courses
        public async Task<IActionResult> Index()
        {
            var faculty = await GetCurrentFacultyAsync();
            if (faculty == null) return RedirectToAction("Login", "Account");

            var myCourses = await _context.CourseOfferings
                .Include(c => c.Course)
                .Include(c => c.Semester)
                .Include(c => c.Enrollments)
                .Where(c => c.InstructorId == faculty.FacultyId)
                .OrderByDescending(c => c.Semester.StartDate)
                .ToListAsync();

            return View(myCourses);
        }

        // 2. Manage Grades (Detailed View)
        public async Task<IActionResult> ManageGrades(int id)
        {
            var faculty = await GetCurrentFacultyAsync();
            if (faculty == null) return Unauthorized();

            var offering = await _context.CourseOfferings
                .Include(c => c.Course)
                .Include(c => c.Semester)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.CourseOfferingId == id);

            if (offering == null || offering.InstructorId != faculty.FacultyId)
            {
                return Unauthorized("You are not the instructor for this course.");
            }

            return View(offering);
        }

        // 3. Submit Detailed Grade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGrade(int enrollmentStudentId, int enrollmentCourseOfferingId, 
            double? assign, double? quiz, double? project, double? mid, double? final)
        {
            var faculty = await GetCurrentFacultyAsync();
            
            var enrollment = await _context.Enrollments
                .Include(e => e.CourseOffering)
                .FirstOrDefaultAsync(e => e.StudentId == enrollmentStudentId && e.CourseOfferingId == enrollmentCourseOfferingId);

            if (enrollment == null) return NotFound();

            if (enrollment.CourseOffering.InstructorId != faculty?.FacultyId)
            {
                return Unauthorized();
            }

            // Update Components
            enrollment.AssignmentScore = assign;
            enrollment.QuizScore = quiz;
            enrollment.ProjectScore = project;
            enrollment.MidScore = mid;
            enrollment.FinalScore = final;

            // Calculate Total Weighted Score
            // Weights: Assign(10%), Quiz(10%), Project(10%), Mid(20%), Final(50%)
            double total = 0;
            total += (assign ?? 0) * 0.10;
            total += (quiz ?? 0) * 0.10;
            total += (project ?? 0) * 0.10;
            total += (mid ?? 0) * 0.20;
            total += (final ?? 0) * 0.50;

            enrollment.TotalScore = Math.Round(total, 1);
            enrollment.Grade = CalculateLetterGrade(enrollment.TotalScore.Value);

            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(ManageGrades), new { id = enrollmentCourseOfferingId });
        }

        private string CalculateLetterGrade(double total)
        {
            if (total >= 90) return "A";
            if (total >= 85) return "A-";
            if (total >= 80) return "B+";
            if (total >= 75) return "B";
            if (total >= 70) return "B-";
            if (total >= 65) return "C+";
            if (total >= 60) return "C";
            if (total >= 55) return "D";
            return "F";
        }

        // 4. Take Attendance UI
        public async Task<IActionResult> Attendance(int id)
        {
            var faculty = await GetCurrentFacultyAsync();
            if (faculty == null) return Unauthorized();

            var offering = await _context.CourseOfferings
                .Include(c => c.Course)
                .Include(c => c.Semester)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.CourseOfferingId == id);

            if (offering == null || offering.InstructorId != faculty.FacultyId)
            {
                return Unauthorized();
            }

            ViewBag.Date = DateTime.Today.ToString("yyyy-MM-dd");
            return View(offering);
        }

        // 5. Submit Attendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAttendance(int courseOfferingId, DateTime date, Dictionary<int, bool> status)
        {
            var faculty = await GetCurrentFacultyAsync();
            var offering = await _context.CourseOfferings.FindAsync(courseOfferingId);
            
            if (offering == null || offering.InstructorId != faculty?.FacultyId) 
                return Unauthorized();

            // Convert date to UTC for PostgreSQL
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            foreach (var studentId in status.Keys)
            {
                bool isPresent = status[studentId];

                // Check if already exists for this date
                var existing = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.CourseOfferingId == courseOfferingId && a.StudentId == studentId && a.Date == date);

                if (existing != null)
                {
                    existing.IsPresent = isPresent;
                }
                else
                {
                    _context.Attendances.Add(new Attendance
                    {
                        CourseOfferingId = courseOfferingId,
                        StudentId = studentId,
                        Date = date,
                        IsPresent = isPresent
                    });
                }
            }
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Attendance Saved Successfully";
            return RedirectToAction(nameof(Attendance), new { id = courseOfferingId });
        }
    }
}
