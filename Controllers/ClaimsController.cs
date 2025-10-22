using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace st10440926_poeparttwo.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly string _jsonPath = Path.Combine("App_Data", "claims.json");
        private readonly string _uploadRoot = "upload"; // folder in MVC app
        private readonly string _key = "POE2025_SECURE_KEY"; // encryption key

        // Lecturer Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Claim submission form (GET)
        [HttpGet]
        public IActionResult Create() => View();

        // Claim submission handler (POST)
        [HttpPost]
        public IActionResult Create(ClaimModel model, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(model);

            //  Ensure main upload folder exists
            if (!Directory.Exists(_uploadRoot))
                Directory.CreateDirectory(_uploadRoot);

            //  Ensure subfolders exist (original & encrypted)
            string originalFolder = Path.Combine(_uploadRoot, "original");
            string encryptedFolder = Path.Combine(_uploadRoot, "encrypted");

            if (!Directory.Exists(originalFolder))
                Directory.CreateDirectory(originalFolder);
            if (!Directory.Exists(encryptedFolder))
                Directory.CreateDirectory(encryptedFolder);

            // If file uploaded, save and encrypt
            if (file != null)
            {
                string originalPath = Path.Combine(originalFolder, file.FileName);
                string encryptedPath = Path.Combine(encryptedFolder, file.FileName + ".enc");

                // Save the unencrypted version
                using (var fs = new FileStream(originalPath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }

                // Encrypt and save encrypted version using your helper
                EncryptionHelper.EncryptFile(originalPath, encryptedPath);

                model.FileName = file.FileName;
            }
            else
            {
                // optional: block submission if no file uploaded
                ModelState.AddModelError("FileName", "Please attach a supporting document before submitting.");
                return View(model);
            }

            // ✅ Save claim data to JSON
            var claims = LoadClaims();
            claims.Add(model);
            SaveClaims(claims);

            TempData["Message"] = "Claim submitted successfully! File stored securely.";
            return RedirectToAction("ViewAll");
        }

        // Show all claims (ViewAll page)
        public IActionResult ViewAll()
        {
            var claims = LoadClaims();
            return View(claims);
        }

        // ---------------- Encryption ----------------
        private byte[] EncryptFile(byte[] data, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
            return aes.IV.Concat(encrypted).ToArray();
        }

        // ---------------- JSON File Handling ----------------
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

