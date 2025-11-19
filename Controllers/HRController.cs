using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            var lecturer = LecturerStorage.GetLecturer(username);
            return View(lecturer);
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
            var user = UserStorage.LoadUsers().FirstOrDefault(u => u.Username == username);
            return View(user);
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
        // SET RATE
        // =====================
        [HttpGet]
        public IActionResult SetRate() => View();

        [HttpPost]
        public IActionResult SetRate(decimal rate)
        {
            HourlyRateStorage.SaveRate(rate);
            return RedirectToAction("Index");
        }

        // ==========================================================
        // ⭐⭐⭐ GENERATE LECTURER REPORT (PDF)
        // ==========================================================
        public IActionResult GenerateLecturerReport()
        {
            var lecturers = LecturerStorage.LoadLecturers();

            byte[] pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Contract Monthly Claim System")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Username").Bold();
                            header.Cell().Text("Full Name").Bold();
                            header.Cell().Text("Email").Bold();
                            header.Cell().Text("Hourly Rate").Bold();
                        });

                        foreach (var l in lecturers)
                        {
                            table.Cell().Text(l.Username);
                            table.Cell().Text(l.FullName);
                            table.Cell().Text(l.Email);
                            table.Cell().Text("R " + l.HourlyRate);
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", "LecturerReport.pdf");
        }

        // ==========================================================
        // ⭐⭐⭐ GENERATE INVOICE FOR A SPECIFIC LECTURER
        // ==========================================================
        public IActionResult GenerateInvoice(string username)
        {
            var lecturer = LecturerStorage.GetLecturer(username);
            if (lecturer == null)
                return NotFound();

            var claims = ClaimStorage.LoadClaims()
                .Where(c => c.LecturerName == lecturer.FullName)
                .ToList();

            byte[] pdf = InvoiceGenerator.Generate(lecturer, claims);

            string fileName = $"Invoice_{lecturer.FullName.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd}.pdf";

            return File(pdf, "application/pdf", fileName);
        }
    }
}
