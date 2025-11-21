using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Data;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace st10440926_poeparttwo.Controllers
{
    public class ClaimsController : Controller
    {
        // FIXED: Upload root now matches your actual folder structure
        private readonly string _uploadRoot = Path.Combine("upload");

        private readonly string _key = "POE2025_SECURE_KEY";
        private readonly ApplicationDbContext _db;

        public ClaimsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Lecturer Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Claim submission (GET)
        [HttpGet]
        public IActionResult Create() => View();

        // Claim submission (POST)
        [HttpPost]
        public IActionResult Create(ClaimModel model, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Ensure upload folders exist
            if (!Directory.Exists(_uploadRoot))
                Directory.CreateDirectory(_uploadRoot);

            string originalFolder = Path.Combine(_uploadRoot, "original");
            string encryptedFolder = Path.Combine(_uploadRoot, "encrypted");

            if (!Directory.Exists(originalFolder))
                Directory.CreateDirectory(originalFolder);

            if (!Directory.Exists(encryptedFolder))
                Directory.CreateDirectory(encryptedFolder);

            // Save + encrypt file
            if (file != null)
            {
                string originalPath = Path.Combine(originalFolder, file.FileName);
                string encryptedPath = Path.Combine(encryptedFolder, file.FileName + ".enc");

                using (var fs = new FileStream(originalPath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }

                EncryptionHelper.EncryptFile(originalPath, encryptedPath);

                model.FileName = file.FileName;
            }
            else
            {
                ModelState.AddModelError("FileName", "Please attach a supporting document.");
                return View(model);
            }

            // Save to SQL
            _db.Claims.Add(model);
            _db.SaveChanges();

            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction("ViewAll");
        }

        // View all claims
        public IActionResult ViewAll()
        {
            var claims = _db.Claims.ToList();
            return View(claims);
        }

        // ============================
        // ⭐ VIEW FILE IN BROWSER
        // ============================
        [HttpGet]
        public IActionResult ViewFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid file name.");

            string filePath = Path.Combine(_uploadRoot, "original", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var bytes = System.IO.File.ReadAllBytes(filePath);

            // Try to detect content type
            string contentType =
                fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" :
                fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ? "image/jpeg" :
                fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ? "image/jpeg" :
                fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" :
                fileName.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ? "image/webp" :
                "application/octet-stream";

            return File(bytes, contentType);
        }

        // ============================
        // ⭐ DOWNLOAD FILE
        // ============================
        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid file name.");

            string filePath = Path.Combine(_uploadRoot, "original", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        // Encryption helper
        private byte[] EncryptFile(byte[] data, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
            return aes.IV.Concat(encrypted).ToArray();
        }
    }
}
