using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace st10440926_poeparttwo.Controllers
{
    public class LecturerController : Controller
    {
        private readonly string _jsonPath = Path.Combine("App_Data", "claims.json");
        private readonly string _uploadRoot = "upload"; // Folder where files are stored
        private readonly string _key = "POE2025_SECURE_KEY"; // encryption key

        public IActionResult Index()
        {
            var claims = LoadClaims();
            return View(claims.Where(c => c.Status != "Approved").ToList());
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(ClaimModel model, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (file != null)
            {
                // Ensure folder exists
                Directory.CreateDirectory(Path.Combine(_uploadRoot, "original"));
                Directory.CreateDirectory(Path.Combine(_uploadRoot, "encrypted"));

                string originalPath = Path.Combine(_uploadRoot, "original", file.FileName);
                string encryptedPath = Path.Combine(_uploadRoot, "encrypted", file.FileName + ".enc");

                // Save normal file
                using (var fs = new FileStream(originalPath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }

                // Encrypt and save encrypted version
                var fileBytes = System.IO.File.ReadAllBytes(originalPath);
                var encrypted = EncryptFile(fileBytes, _key);
                System.IO.File.WriteAllBytes(encryptedPath, encrypted);

                model.FileName = file.FileName;
            }

            var claims = LoadClaims();
            claims.Add(model);
            SaveClaims(claims);

            TempData["Message"] = "Claim submitted successfully! File stored securely.";
            return RedirectToAction("ViewAll");
        }

        public IActionResult ViewAll()
        {
            var claims = LoadClaims();
            return View(claims);
        }

        // ---------------- ENCRYPTION ----------------
        private byte[] EncryptFile(byte[] data, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
            return aes.IV.Concat(encrypted).ToArray();
        }

        // ---------------- JSON LOAD/SAVE ----------------
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
