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
            // Save login user
            UserStorage.AddUser(new UserModel
            {
                Username = Username,
                Password = Password,
                Role = "Lecturer"
            });

            // Save lecturer profile
            LecturerStorage.AddLecturer(new LecturerProfile
            {
                Username = Username,
                FullName = FullName,
                HourlyRate = HourlyRate
            });

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

            // Update user login username
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
