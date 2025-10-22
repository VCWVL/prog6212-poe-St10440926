using Microsoft.AspNetCore.Mvc;
using st10440926_poeparttwo.Models;
using System.Text.Json;

namespace st10440926_poeparttwo.Controllers
{
    public class ManagerController : Controller
    {
        private readonly string _jsonPath = Path.Combine("App_Data", "claims.json");

        public IActionResult Index()
        {
            var claims = LoadClaims().Where(c => c.Status == "Verified").ToList();
            return View(claims);
        }

        public IActionResult Approve(string id)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
                claim.Status = "Approved";
            SaveClaims(claims);
            return RedirectToAction("Index");
        }

        public IActionResult Reject(string id)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
                claim.Status = "Rejected";
            SaveClaims(claims);
            return RedirectToAction("Index");
        }

        private List<ClaimModel> LoadClaims()
        {
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

