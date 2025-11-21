using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Services;
using st10440926_poeparttwo.Data;
using System.Security.Cryptography;
using System.Text;

namespace st10440926_poeparttwo.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly string _uploadRoot = "upload";
        private readonly string _key = "POE2025_SECURE_KEY";

        public LecturerController(ApplicationDbContext db)
        {
            _db = db;
        }

        // LECTURER DASHBOARD 
  
        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Role");

            var myClaims = _db.Claims
                .Where(c => c.LecturerUsername == username && c.Status != "Approved")
                .ToList();

            return View(myClaims);
        }

       
        // CREATE CLAIM (GET)
       
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
                LecturerUsername = username
            };

            return View(model);
        }

        
        // CREATE CLAIM (POST) 
      
        [HttpPost]
        public IActionResult Create(ClaimModel model, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.LecturerUsername = HttpContext.Session.GetString("Username");

            // Upload , encrypt file
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

            
            //  AUTO-APPROVE LOGIC FOR LECTURERS
        
            if (model.HoursWorked >= 1 && model.HoursWorked <= 150)
            {
                model.Status = "Approved";
            }
            else
            {
                model.Status = "Pending";  
            }

            // SAVE TO SQL
            _db.Claims.Add(model);
            _db.SaveChanges();

            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction("ViewAll");
        }

        
        // VIEW ALL — ONLY MY CLAIMS
        
        public IActionResult ViewAll()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Role");

            var myClaims = _db.Claims
                .Where(c => c.LecturerUsername == username)
                .ToList();

            return View(myClaims);
        }

        
        // ENCRYPTION
       
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
