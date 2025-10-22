using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;

namespace st10440926_poeparttwo.Controllers
{
    public class RoleController : Controller
    {
        private readonly Dictionary<string, string> _roles = new()
        {
            { "Lecturer", "lect123" },
            { "Coordinator", "coord123" },
            { "Manager", "admin123" }
        };

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Role) || string.IsNullOrWhiteSpace(user.Password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            if (_roles.TryGetValue(user.Role, out string correctPassword) && user.Password == correctPassword)
            {
                HttpContext.Session.SetString("UserRole", user.Role);
                return RedirectToAction("Index", user.Role);
            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
