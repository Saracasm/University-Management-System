using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WP_project.Data;
using WP_project.Models;
using WP_project.ViewModels;

namespace WP_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UniversityDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, UniversityDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .Include(u => u.Student)
                .Include(u => u.Faculty)
                .ToListAsync();
            return View(users);
        }

        // GET: CreateStudent
        public IActionResult CreateStudent()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Create Identity User
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = "Student",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");

                    // 2. Create Student Profile
                    var student = new Student
                    {
                        Name = model.FullName,
                        Email = model.Email,
                        RollNumber = model.RollNumber,
                        DepartmentId = model.DepartmentId
                        // ApplicationUserId is linked via relationship, but we need to set the FK on User or link here.
                        // Since ApplicationUser is the Principal in the 1:1 defined in DbContext, 
                        // we actually need to update the User with the StudentId OR set the User navigation on Student.
                        // Let's see how EF handles it. Ideally: Save student, get ID, update user.
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    // Link them
                    user.StudentId = student.StudentId;
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", model.DepartmentId);
            return View(model);
        }

        // GET: CreateFaculty
        public IActionResult CreateFaculty()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFaculty(CreateFacultyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = "Faculty",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Faculty");

                    var faculty = new Faculty
                    {
                        Name = model.FullName,
                        Email = model.Email,
                        DepartmentId = model.DepartmentId
                    };

                    _context.Faculties.Add(faculty);
                    await _context.SaveChangesAsync();

                    user.FacultyId = faculty.FacultyId;
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", model.DepartmentId);
            return View(model);
        }
    }
}

