using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Data;   // ⭐ ADDED
using System.Linq;

namespace st10440926_poeparttwo.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _db;   // ⭐ ADDED

        public CoordinatorController(ApplicationDbContext db)  // ⭐ ADDED
        {
            _db = db;
        }

        // =============================
        // VIEW PENDING CLAIMS
        // =============================
        public IActionResult Index()
        {
            var claims = _db.Claims
                .Where(c => c.Status == "Pending")
                .ToList();                           // ⭐ SQL REPLACEMENT

            return View(claims);
        }

        // =============================
        // VERIFY CLAIM
        // =============================
        public IActionResult Verify(string id)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);  // ⭐ SQL
            if (claim != null)
            {
                claim.Status = "Verified";
                _db.SaveChanges();                                   // ⭐ SQL SAVE
            }

            return RedirectToAction("Index");
        }

        // =============================
        // REJECT CLAIM
        // =============================
        public IActionResult Reject(string id)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);  // ⭐ SQL
            if (claim != null)
            {
                claim.Status = "Rejected";
                _db.SaveChanges();                                   // ⭐ SQL SAVE
            }

            return RedirectToAction("Index");
        }
    }
}
