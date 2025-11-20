using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Services;   // ⭐ REQUIRED to access LecturerStorage
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace st10440926_poeparttwo.Controllers
{
    public class LecturerController : Controller
    {
        private readonly string _jsonPath = Path.Combine("App_Data", "claims.json");
        private readonly string _uploadRoot = "upload";
        private readonly string _key = "POE2025_SECURE_KEY";

        // ------------------------------------------------------
        // LECTURER DASHBOARD — SHOW ONLY MY (NOT APPROVED) CLAIMS
        // ------------------------------------------------------
        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Role");

            var claims = LoadClaims();

            // ⭐ FILTER: Only logged-in lecturer + not approved
            var myClaims = claims
                .Where(c => c.LecturerUsername == username && c.Status != "Approved")
                .ToList();

            return View(myClaims);
        }

        // ------------------------------------------------------
        // CREATE CLAIM (GET) — AUTO-FILL LECTURER INFO
        // ------------------------------------------------------
        [HttpGet]
        public IActionResult Create()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Role");

            var profile = LecturerStorage.GetLecturer(username);

            if (profile == null)
            {
                TempData["Error"] = "Lecturer profile not found. Contact HR.";
                return RedirectToAction("Index");
            }

            var model = new ClaimModel
            {
                LecturerName = profile.FullName,
                HourlyRate = (double)profile.HourlyRate,
                LecturerUsername = username  // ⭐ REQUIRED FIX
            };

            return View(model);
        }

        // ------------------------------------------------------
        // CREATE CLAIM (POST)
        // ------------------------------------------------------
        [HttpPost]
        public IActionResult Create(ClaimModel model, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ⭐ Store logged-in lecturer on the claim
            model.LecturerUsername = HttpContext.Session.GetString("Username");

            // Handle file upload & encryption
            if (file != null)
            {
                Directory.CreateDirectory(Path.Combine(_uploadRoot, "original"));
                Directory.CreateDirectory(Path.Combine(_uploadRoot, "encrypted"));

                string originalPath = Path.Combine(_uploadRoot, "original", file.FileName);
                string encryptedPath = Path.Combine(_uploadRoot, "encrypted", file.FileName + ".enc");

                using (var fs = new FileStream(originalPath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }

                var fileBytes = System.IO.File.ReadAllBytes(originalPath);
                var encrypted = EncryptFile(fileBytes, _key);
                System.IO.File.WriteAllBytes(encryptedPath, encrypted);

                model.FileName = file.FileName;
            }

            // Save claim
            var claims = LoadClaims();
            claims.Add(model);
            SaveClaims(claims);

            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction("ViewAll");
        }

        // ------------------------------------------------------
        // VIEW ALL — ONLY MY CLAIMS
        // ------------------------------------------------------
        public IActionResult ViewAll()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Role");

            var claims = LoadClaims();

            // ⭐ FILTER: Only logged-in lecturer’s claims
            var myClaims = claims
                .Where(c => c.LecturerUsername == username)
                .ToList();

            return View(myClaims);
        }

        // ------------------------------------------------------
        // ENCRYPTION
        // ------------------------------------------------------
        private byte[] EncryptFile(byte[] data, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

            return aes.IV.Concat(encrypted).ToArray();
        }

        // ------------------------------------------------------
        // JSON LOAD / SAVE
        // ------------------------------------------------------
        private List<ClaimModel> LoadClaims()
        {
            if (!Directory.Exists("App_Data"))
                Directory.CreateDirectory("App_Data");

            if (!System.IO.File.Exists(_jsonPath))
                return new List<ClaimModel>();

            string json = System.IO.File.ReadAllText(_jsonPath);
            return JsonSerializer.Deserialize<List<ClaimModel>>(json) ?? new List<ClaimModel>();
        }

        private void SaveClaims(List<ClaimModel> claims)
        {
            string json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }
    }
}
