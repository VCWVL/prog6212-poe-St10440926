using System.Text.Json;
using st10440926_poeparttwo.Models;

namespace st10440926_poeparttwo.Services
{
    public static class ClaimStorage
    {
        private static readonly string FilePath = Path.Combine("App_Data", "claims.json");

        public static List<ClaimModel> LoadClaims()
        {
            if (!File.Exists(FilePath))
                return new List<ClaimModel>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<ClaimModel>>(json)
                   ?? new List<ClaimModel>();
        }

        public static void SaveClaims(List<ClaimModel> claims)
        {
            var json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void AddClaim(ClaimModel claim)
        {
            var list = LoadClaims();
            list.Add(claim);
            SaveClaims(list);
        }
    }
}
