using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Services;
using st10440926_poeparttwo.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace st10440926_poeparttwo.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HRController(ApplicationDbContext db)
        {
            _db = db;
        }

        
        // HR DASHBOARD
       
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var model = new HRDashboardViewModel
            {
                Users = _db.Users.ToList(),
                Lecturers = LecturerStorage.LoadLecturers(),
                Claims = _db.Claims.ToList(),   
                StandardRate = HourlyRateStorage.LoadRate()
            };

            return View(model);
        }

   
        // CREATE GENERAL USER
    
        [HttpGet]
        public IActionResult CreateUser()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(string Username, string FullName, string Email, string Role, decimal? HourlyRate, string Password)
        {
            var newUser = new UserModel
            {
                Username = Username,
                Password = Password,
                Role = Role
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            
            if (Role == "Lecturer")
            {
                LecturerStorage.AddLecturer(new LecturerProfile
                {
                    Username = Username,
                    FullName = FullName,
                    Email = Email,
                    HourlyRate = HourlyRate ?? 0
                });
            }

            return RedirectToAction("Index");
        }

       
        // EDIT USER
     
        [HttpGet]
        public IActionResult EditUser(string username)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(string OriginalUsername, string Username, string Role, string Password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == OriginalUsername);

            if (user != null)
            {
                user.Username = Username;
                user.Role = Role;

                if (!string.IsNullOrWhiteSpace(Password))
                    user.Password = Password;

                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

       
        // DELETE USER
    
        public IActionResult DeleteUser(string username)
        {
            if (HttpContext.Session.GetString("Username") == username)
            {
                TempData["Error"] = "You cannot delete the currently logged-in user.";
                return RedirectToAction("Index");
            }

            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }

            LecturerStorage.DeleteLecturer(username);

            return RedirectToAction("Index");
        }

        // LECTURER MANAGEMENT
      
        public IActionResult CreateLecturer() => View();

        [HttpGet]
        public IActionResult EditLecturer(string username)
        {
            var lecturer = LecturerStorage.GetLecturer(username);
            return View(lecturer);
        }

        [HttpPost]
        public IActionResult EditLecturer(string OriginalUsername, string Username, string FullName, string Email, decimal HourlyRate)
        {
            var lecturer = LecturerStorage.GetLecturer(OriginalUsername);

            if (lecturer == null)
                return NotFound();

            lecturer.Username = Username;
            lecturer.FullName = FullName;
            lecturer.Email = Email;
            lecturer.HourlyRate = HourlyRate;

            LecturerStorage.UpdateLecturer(OriginalUsername, lecturer);

            return RedirectToAction("Index");
        }

       
        // DELETE LECTURER
       
        public IActionResult DeleteLecturer(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return NotFound();

            LecturerStorage.DeleteLecturer(username);

            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        
        // GENERATE APPROVED REPORT 
        
        public IActionResult GenerateApprovedReport(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return NotFound();

            var lecturer = LecturerStorage.GetLecturer(username);
            if (lecturer == null)
                return NotFound("Lecturer not found.");

            var approvedClaims = _db.Claims
                .Where(c => c.LecturerUsername == username && c.Status == "Approved")
                .ToList();

            if (!approvedClaims.Any())
            {
                TempData["Error"] = "No approved claims available for this lecturer.";
                return RedirectToAction("ApprovedClaims");
            }

            var pdfBytes = ReportGenerator.GenerateLecturerReport(lecturer, approvedClaims);

            return File(pdfBytes, "application/pdf", $"{username}_ApprovedReport.pdf");
        }

        
        // CLAIM VIEWS 
     
        public IActionResult PendingClaims()
        {
            var pendingClaims = _db.Claims
                .Where(c => c.Status == "Pending")
                .ToList();

            return View(pendingClaims);
        }

        public IActionResult VerifiedClaims()
        {
            var verifiedClaims = _db.Claims
                .Where(c => c.Status == "Verified")
                .ToList();

            return View(verifiedClaims);
        }

        public IActionResult ApprovedClaims()
        {
            var approvedClaims = _db.Claims
                .Where(c => c.Status == "Approved")
                .ToList();

            return View(approvedClaims);
        }

        public IActionResult ManageUsers() =>
            View(_db.Users.ToList());
    }
}
