using Microsoft.AspNetCore.Mvc;
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

            var users = UserStorage.LoadUsers();
            var lecturers = LecturerStorage.LoadLecturers();
            var rate = HourlyRateStorage.LoadRate();

            var model = new HRDashboardViewModel
            {
                Users = users,
                LecturerProfiles = lecturers,
                StandardRate = rate
            };

            return View(model);
        }

        // =====================
        // CREATE LECTURER (GET)
        // =====================
        [HttpGet]
        public IActionResult CreateLecturer()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View();
        }

        // =====================
        // CREATE LECTURER (POST)
        // =====================
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
        // CREATE GENERAL USER (GET)
        // =====================
        [HttpGet]
        public IActionResult CreateUser()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View();
        }

        // =====================
        // CREATE GENERAL USER (POST)
        // =====================
        [HttpPost]
        public IActionResult CreateUser(string Username, string FullName, string Email, string Role, decimal? HourlyRate, string Password)
        {
            // Add login credentials
            UserStorage.AddUser(new UserModel
            {
                Username = Username,
                Password = Password,
                Role = Role
            });

            // If user is a lecturer, create lecturer profile also
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
        // EDIT LECTURER (GET)
        // =====================
        [HttpGet]
        public IActionResult EditLecturer(string username)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var lecturer = LecturerStorage.GetLecturer(username);
            return View(lecturer);
        }

        // =====================
        // EDIT LECTURER (POST)
        // =====================
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

            // Update login credentials
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
        // EDIT GENERAL USER (GET)
        // =====================
        [HttpGet]
        public IActionResult EditUser(string username)
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            var user = UserStorage.LoadUsers().FirstOrDefault(u => u.Username == username);
            return View(user);
        }

        // =====================
        // EDIT GENERAL USER (POST)
        // =====================
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
        // DELETE GENERAL USER
        // =====================
        public IActionResult DeleteUser(string username)
        {
            // Prevent deleting the user currently logged in
            if (HttpContext.Session.GetString("Username") == username)
            {
                TempData["Error"] = "You cannot delete the currently logged-in user.";
                return RedirectToAction("Index");
            }

            // Remove login user
            UserStorage.DeleteUser(username);

            // If they were a lecturer, remove profile too
            LecturerStorage.DeleteLecturer(username);

            return RedirectToAction("Index");
        }

        // =====================
        // SET HOURLY RATE (GET)
        // =====================
        [HttpGet]
        public IActionResult SetRate()
        {
            if (HttpContext.Session.GetString("UserRole") != "HR")
                return RedirectToAction("Login", "Role");

            return View();
        }

        // =====================
        // SET HOURLY RATE (POST)
        // =====================
        [HttpPost]
        public IActionResult SetRate(decimal rate)
        {
            HourlyRateStorage.SaveRate(rate);
            return RedirectToAction("Index");
        }
    }
}
