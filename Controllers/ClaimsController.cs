using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Data;
using System.Security.Cryptography;
using System.Text;

namespace st10440926_poeparttwo.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly string _uploadRoot = Path.Combine("upload");
        private readonly string _key = "POE2025_SECURE_KEY";
        private readonly ApplicationDbContext _db;

        public ClaimsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(ClaimModel model, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!Directory.Exists(_uploadRoot))
                Directory.CreateDirectory(_uploadRoot);

            string originalFolder = Path.Combine(_uploadRoot, "original");
            string encryptedFolder = Path.Combine(_uploadRoot, "encrypted");

            if (!Directory.Exists(originalFolder))
                Directory.CreateDirectory(originalFolder);

            if (!Directory.Exists(encryptedFolder))
                Directory.CreateDirectory(encryptedFolder);

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

            // lecturers save via LecturerController
            _db.Claims.Add(model);
            _db.SaveChanges();

            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction("ViewAll");
        }

        public IActionResult ViewAll()
        {
            var claims = _db.Claims.ToList();
            return View(claims);
        }

        [HttpGet]
        public IActionResult ViewFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid file name.");

            string filePath = Path.Combine(_uploadRoot, "original", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var bytes = System.IO.File.ReadAllBytes(filePath);

            string contentType =
                fileName.EndsWith(".pdf") ? "application/pdf" :
                fileName.EndsWith(".jpg") ? "image/jpeg" :
                fileName.EndsWith(".jpeg") ? "image/jpeg" :
                fileName.EndsWith(".png") ? "image/png" :
                fileName.EndsWith(".webp") ? "image/webp" :
                "application/octet-stream";

            return File(bytes, contentType);
        }

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
    }
}
