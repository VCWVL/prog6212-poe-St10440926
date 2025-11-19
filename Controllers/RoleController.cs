using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Services;

namespace st10440926_poeparttwo.Controllers
{
    public class RoleController : Controller
    {
        private readonly Dictionary<string, string> _roles = new()
        {
            { "Lecturer", "lect123" },
            { "Coordinator", "coord123" },
            { "Manager", "admin123" },
            { "HR", "hr123" }
        };

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            // Validate fields
            if (string.IsNullOrWhiteSpace(user.Role) ||
                string.IsNullOrWhiteSpace(user.Username) ||
                string.IsNullOrWhiteSpace(user.Password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            // ---------------------------------------
            // 🔐 HR LOGIN (Hard-coded)
            // ---------------------------------------
            if (user.Role == "HR")
            {
                if (user.Username == "hr" && user.Password == "hr123")
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserRole", user.Role);
                    return RedirectToAction("Index", "HR");
                }

                ViewBag.Error = "Invalid HR login details.";
                return View();
            }

            // ---------------------------------------
            // 📌 LECTURER LOGIN (from UserStorage.json)
            // ---------------------------------------
            var users = UserStorage.LoadUsers();

            var match = users.FirstOrDefault(u =>
                u.Username == user.Username &&
                u.Password == user.Password &&
                u.Role == user.Role);

            if (match != null)
            {
                HttpContext.Session.SetString("Username", match.Username);
                HttpContext.Session.SetString("UserRole", match.Role);

                return match.Role switch
                {
                    "Lecturer" => RedirectToAction("Index", "Lecturer"),
                    "Coordinator" => RedirectToAction("Index", "Coordinator"),
                    "Manager" => RedirectToAction("Index", "Manager"),
                    _ => RedirectToAction("Login")
                };
            }

            // ---------------------------------------
            // 📌 COORDINATOR + MANAGER from dictionary
            // ---------------------------------------
            if (_roles.TryGetValue(user.Role, out string correctPassword)
                && user.Password == correctPassword)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                return user.Role switch
                {
                    "Coordinator" => RedirectToAction("Index", "Coordinator"),
                    "Manager" => RedirectToAction("Index", "Manager"),
                    _ => RedirectToAction("Login")
                };
            }

            // ---------------------------------------
            // ❌ If login fails
            // ---------------------------------------
            ViewBag.Error = "Invalid login details.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
