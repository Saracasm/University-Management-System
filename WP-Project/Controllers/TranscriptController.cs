using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WP_project.Data;
using WP_project.Models;

namespace WP_project.Controllers
{
    [Authorize(Roles = "Student")]
    public class TranscriptController : Controller
    {
        private readonly UniversityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TranscriptController(UniversityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            
            // Fetch all enrollments with grades
            var completedCourses = await _context.Enrollments
                .Include(e => e.CourseOffering)
                .ThenInclude(co => co.Course)
                .Include(e => e.CourseOffering)
                .ThenInclude(co => co.Semester)
                .Where(e => e.StudentId == studentId && e.Grade != null) // Only graded courses
                .OrderBy(e => e.CourseOffering.Semester.StartDate)
                .ToListAsync();

            // Calculate GPA
            var transcriptViewModel = new TranscriptViewModel
            {
                StudentName = user.FullName,
                RollNumber = (await _context.Students.FindAsync(studentId))?.RollNumber ?? "N/A",
                Semesters = new List<SemesterTranscriptViewModel>()
            };

            // Group by Semester
            var grouped = completedCourses.GroupBy(c => c.CourseOffering.Semester.Name);

            double totalPoints = 0;
            double totalCredits = 0;

            foreach (var group in grouped)
            {
                var semesterView = new SemesterTranscriptViewModel
                {
                    SemesterName = group.Key,
                    Courses = new List<TranscriptCourseViewModel>()
                };

                foreach (var course in group)
                {
                    double points = GradeToPoint(course.Grade);
                    int credits = course.CourseOffering.Course.Credits;

                    semesterView.Courses.Add(new TranscriptCourseViewModel
                    {
                        Code = course.CourseOffering.Course.Code,
                        Title = course.CourseOffering.Course.Title,
                        Credits = credits,
                        Grade = course.Grade,
                        Points = points
                    });

                    if (course.Grade != "F") // Failed courses usually count in GPA but earn 0 credits. 
                    {
                        // Standard CGPA: Include F in Credits Attempted? 
                        // Let's assume standard: F counts as 0 points but counts towards total credits attempted.
                        totalPoints += (points * credits);
                        totalCredits += credits;
                    }
                    else
                    {
                        // If F, points = 0, but we add credits to total?
                        // Simplified: F = 0 points, and it adds to denominator.
                        totalPoints += (0 * credits);
                        totalCredits += credits;
                    }
                }
                
                // Semester GPA
                double semPoints = semesterView.Courses.Sum(c => c.Points * c.Credits);
                double semCredits = semesterView.Courses.Sum(c => c.Credits);
                semesterView.SemesterGPA = semCredits > 0 ? Math.Round(semPoints / semCredits, 2) : 0.0;

                transcriptViewModel.Semesters.Add(semesterView);
            }

            transcriptViewModel.CGPA = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0.0;

            return View(transcriptViewModel);
        }

        private double GradeToPoint(string? grade)
        {
            return grade switch
            {
                "A" => 4.0,
                "A-" => 3.7,
                "B+" => 3.3,
                "B" => 3.0,
                "B-" => 2.7,
                "C+" => 2.3,
                "C" => 2.0,
                "D" => 1.0,
                "F" => 0.0,
                _ => 0.0
            };
        }
    }
}

