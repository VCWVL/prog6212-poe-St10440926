using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Data;   
using System.Linq;

namespace st10440926_poeparttwo.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _db;   

        public CoordinatorController(ApplicationDbContext db) 
        {
            _db = db;
        }

        
        // VIEW PENDING CLAIMS
   
        public IActionResult Index()
        {
            var claims = _db.Claims
                .Where(c => c.Status == "Pending")
                .ToList();                           

            return View(claims);
        }

       
        // VERIFY CLAIM
        
        public IActionResult Verify(string id)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);  
            if (claim != null)
            {
                claim.Status = "Verified";
                _db.SaveChanges();                                  
            }

            return RedirectToAction("Index");
        }

       
        // REJECT CLAIM
     
        public IActionResult Reject(string id)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);  
            if (claim != null)
            {
                claim.Status = "Rejected";
                _db.SaveChanges();                                   
            }

            return RedirectToAction("Index");
        }
    }
}
