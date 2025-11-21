using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using st10440926_poeparttwo.Data;     
using System.Linq;

namespace st10440926_poeparttwo.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _db;   

        public ManagerController(ApplicationDbContext db)
        {
            _db = db;
        }

        
        // MANAGER DASHBOARD
        // Show Verified claims only
       
        public IActionResult Index()
        {
            var claims = _db.Claims
                .Where(c => c.Status == "Verified")   
                .ToList();

            return View(claims);
        }

       
        // APPROVE CLAIM
        
        public IActionResult Approve(string id)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);

            if (claim != null)
            {
                claim.Status = "Approved";   
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
