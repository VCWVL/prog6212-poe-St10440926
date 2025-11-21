using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Data;   
using System.Linq;

namespace st10440926_poeparttwo.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        
        // LOGIN (GET)
        
        [HttpGet]
        public IActionResult Login() => View();

        
        // LOGIN (POST)
        
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

            
            var match = _db.Users.FirstOrDefault(u =>
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
                    "HR" => RedirectToAction("Index", "HR"),
                    _ => RedirectToAction("Login")
                };
            }

            //  Login failed
            ViewBag.Error = "Invalid login details.";
            return View();
        }

        
        // LOGOUT
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
