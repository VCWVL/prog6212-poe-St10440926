using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Services;

namespace st10440926_poeparttwo.Controllers
{
    public class HRController : Controller
    {
        // =====================
        // HR DASHBOARD
        // =====================
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var model = new HRDashboardViewModel
            {
                Users = UserStorage.LoadUsers(),
                Lecturers = LecturerStorage.LoadLecturers(),
                Claims = ClaimStorage.LoadClaims(),
                StandardRate = HourlyRateStorage.LoadRate()
            };

            return View(model);
        }

        // =====================
        // CREATE LECTURER
        // =====================
        [HttpGet]
        public IActionResult CreateLecturer()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View();
        }

        [HttpPost]
        public IActionResult CreateLecturer(string Username, string FullName, decimal HourlyRate, string Password)
        {
            UserStorage.AddUser(new UserModel
            {
                Username = Username,
                Password = Password,
                Role = "Lecturer"
            });

            LecturerStorage.AddLecturer(new LecturerProfile
            {
                Username = Username,
                FullName = FullName,
                HourlyRate = HourlyRate
            });

            return RedirectToAction("Index");
        }

        // =====================
        // CREATE GENERAL USER
        // =====================
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
            UserStorage.AddUser(new UserModel
            {
                Username = Username,
                Password = Password,
                Role = Role
            });

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

        // =====================
        // EDIT LECTURER
        // =====================
        [HttpGet]
        public IActionResult EditLecturer(string username)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View(LecturerStorage.GetLecturer(username));
        }

        [HttpPost]
        public IActionResult EditLecturer(string OriginalUsername, string Username, string FullName, string Email, decimal HourlyRate)
        {
            LecturerStorage.UpdateLecturer(OriginalUsername, new LecturerProfile
            {
                Username = Username,
                FullName = FullName,
                Email = Email,
                HourlyRate = HourlyRate
            });

            UserStorage.UpdateUser(OriginalUsername, Username);

            return RedirectToAction("Index");
        }

        // =====================
        // DELETE LECTURER
        // =====================
        public IActionResult DeleteLecturer(string username)
        {
            LecturerStorage.DeleteLecturer(username);
            UserStorage.DeleteUser(username);
            return RedirectToAction("Index");
        }

        // =====================
        // EDIT USER
        // =====================
        [HttpGet]
        public IActionResult EditUser(string username)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View(UserStorage.LoadUsers().FirstOrDefault(u => u.Username == username));
        }

        [HttpPost]
        public IActionResult EditUser(string OriginalUsername, string Username, string Role, string Password)
        {
            var users = UserStorage.LoadUsers();
            var existing = users.FirstOrDefault(u => u.Username == OriginalUsername);

            if (existing != null)
            {
                existing.Username = Username;
                existing.Role = Role;

                if (!string.IsNullOrWhiteSpace(Password))
                    existing.Password = Password;

                UserStorage.SaveUsers(users);
            }

            return RedirectToAction("Index");
        }

        // =====================
        // DELETE USER
        // =====================
        public IActionResult DeleteUser(string username)
        {
            if (HttpContext.Session.GetString("Username") == username)
            {
                TempData["Error"] = "You cannot delete the currently logged-in user.";
                return RedirectToAction("Index");
            }

            UserStorage.DeleteUser(username);
            LecturerStorage.DeleteLecturer(username);

            return RedirectToAction("Index");
        }

        // =====================
        // SET STANDARD HOURLY RATE
        // =====================
        [HttpGet]
        public IActionResult SetRate()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View();
        }

        [HttpPost]
        public IActionResult SetRate(decimal rate)
        {
            HourlyRateStorage.SaveRate(rate);
            return RedirectToAction("Index");
        }

        // =====================
        // HR: PENDING CLAIMS
        // =====================
        public IActionResult PendingClaims()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var claims = ClaimStorage.LoadClaims()
                .Where(c => c.Status == "Pending")
                .ToList();

            return View(claims);
        }

        // =====================
        // HR: VERIFIED CLAIMS
        // =====================
        public IActionResult VerifiedClaims()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var claims = ClaimStorage.LoadClaims()
                .Where(c => c.Status == "Verified")
                .ToList();

            return View(claims);
        }

        // =====================
        // HR: APPROVED CLAIMS
        // =====================
        public IActionResult ApprovedClaims()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var claims = ClaimStorage.LoadClaims()
                .Where(c => c.Status == "Approved")
                .ToList();

            return View(claims);
        }

        // ==========================================================
        // ⭐ GENERATE REPORT FOR APPROVED CLAIMS OF A LECTURER
        // ==========================================================
        public IActionResult GenerateApprovedReport(string username)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var lecturer = LecturerStorage.GetLecturer(username);
            if (lecturer == null)
                return NotFound("Lecturer not found.");

            var approvedClaims = ClaimStorage.LoadClaims()
                .Where(c => c.LecturerUsername == lecturer.Username && c.Status == "Approved")
                .ToList();

            if (!approvedClaims.Any())
                return NotFound("No approved claims found for this lecturer.");

            var pdf = ReportGenerator.GenerateLecturerReport(lecturer, approvedClaims);

            string fileName = $"ApprovedClaims_{lecturer.FullName.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd}.pdf";

            return File(pdf, "application/pdf", fileName);
        }

        // =====================
        // ⭐ MANAGE ALL USERS
        // =====================
        public IActionResult ManageUsers()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var users = UserStorage.LoadUsers();
            return View(users);
        }
    }
}
