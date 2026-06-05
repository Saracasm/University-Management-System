# University Management System

A full-stack web application for managing university operations — students, faculty, courses, enrollments, grading, attendance, and transcripts — built with ASP.NET Core MVC and PostgreSQL.

---

## Features

### Admin
- Create and manage student and faculty accounts
- Manage departments and semesters
- View all registered users with their roles

### Faculty
- View assigned course offerings
- Mark and manage student attendance
- Enter and update grades (quiz, assignment, mid, project, final)

### Student
- View enrolled courses and schedule
- Check attendance records
- View grades and generated transcript (CGPA)

### General
- Role-based access control (Admin / Faculty / Student)
- Secure authentication via ASP.NET Core Identity
- Course prerequisite management
- Automatic CGPA calculation on transcript

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8.0 MVC |
| Language | C# (.NET 8) |
| Database | PostgreSQL 16+ |
| ORM | Entity Framework Core 8 |
| Auth | ASP.NET Core Identity |
| Frontend | Razor Views, Bootstrap 5, jQuery |
| Validation | jQuery Validation + Unobtrusive |

---

## Project Structure

```
University-Management-System/
├── WP-Project.sln                  # Visual Studio solution
└── WP-Project/
    ├── Controllers/                # Request handlers (one per feature area)
    │   ├── AccountController.cs    # Login / logout
    │   ├── AdminController.cs      # User creation, user list
    │   ├── StudentController.cs    # Student views
    │   ├── FacultyController.cs    # Attendance, grade management
    │   ├── CoursesController.cs    # Course CRUD + prerequisites
    │   ├── CourseOfferingsController.cs
    │   ├── DepartmentsController.cs
    │   ├── SemestersController.cs
    │   ├── TranscriptController.cs
    │   └── HomeController.cs
    ├── Models/                     # Entity classes (EF Core)
    │   ├── ApplicationUser.cs      # Extended Identity user
    │   ├── Student.cs
    │   ├── Faculty.cs
    │   ├── Department.cs
    │   ├── Course.cs
    │   ├── CourseOffering.cs       # Course in a specific semester
    │   ├── Enrollment.cs           # Student ↔ CourseOffering + scores
    │   ├── Attendance.cs
    │   ├── Grade.cs
    │   ├── Prerequisite.cs
    │   ├── Semester.cs
    │   └── Transcript.cs
    ├── ViewModels/                 # Form / display models
    ├── Views/                      # Razor templates (per controller)
    ├── Data/
    │   ├── UniversityDbContext.cs  # EF Core DbContext
    │   └── DbInitializer.cs       # Seeds roles + default admin
    ├── Migrations/                 # EF Core migration history
    ├── wwwroot/                    # Static assets (CSS, JS, libs)
    ├── Program.cs                  # App startup + DI configuration
    ├── appsettings.json            # Connection string + logging config
    └── WP-Project.csproj           # NuGet dependencies
```

---

## Architecture

The app follows the standard **ASP.NET Core MVC** pattern:

```
Browser → Controller → Service/DbContext → PostgreSQL
                ↓
            Razor View → Browser
```

- **Controllers** handle HTTP requests, call EF Core directly, pass data to views.
- **Models** are EF Core entities mapped to PostgreSQL tables.
- **ViewModels** decouple form data from database entities.
- **DbInitializer** runs at startup to seed roles and the default admin account.
- **ASP.NET Core Identity** manages authentication; roles (`Admin`, `Faculty`, `Student`) gate access via `[Authorize(Roles = "...")]`.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [PostgreSQL 16+](https://www.postgresql.org/download/)

---

## How to Run

### 1. Clone the repository

```bash
git clone https://github.com/saracasm/university-management-system.git
cd university-management-system
```

### 2. Configure the database connection

Open `WP-Project/appsettings.json` and update the connection string to match your local PostgreSQL credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=UniversityDB;Username=postgres;Password=YOUR_PASSWORD"
}
```

### 3. Apply database migrations

```bash
cd WP-Project
dotnet restore
dotnet tool install --global dotnet-ef   # skip if already installed
dotnet ef database update
```

This creates the `UniversityDB` database and all tables, and seeds the default admin account.

### 4. Run the application

```bash
dotnet run
```

Then open your browser at **http://localhost:5104**

---

## Default Login

| Role | Email | Password |
|---|---|---|
| Admin | admin@university.com | Admin@123 |

Use the Admin account to create Faculty and Student accounts from the Users panel.

---

## Database Schema (key relationships)

```
Department ──< Course ──< Prerequisite
           ──< Faculty
           ──< Student ──< Enrollment >── CourseOffering ──< Semester
                                    \                    \── Faculty (Instructor)
                                     ──< Attendance
                                     ──< Grade
           ──< Transcript
```

---

## Known Setup Notes

- The app auto-creates the database if it doesn't exist when running `dotnet ef database update`.
- Roles and the admin user are seeded automatically on first startup via `DbInitializer`.
- PostgreSQL must be running before starting the app.
