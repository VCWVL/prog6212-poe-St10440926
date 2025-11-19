using System.Text.Json;

namespace st10440926_poeparttwo.Services
{
    public static class HourlyRateStorage
    {
        private static readonly string FilePath = Path.Combine("App_Data", "hourlyrate.json");

        public static decimal LoadRate()
        {
            if (!File.Exists(FilePath))
                return 0;

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<decimal>(json);
        }

        public static void SaveRate(decimal rate)
        {
            string json = JsonSerializer.Serialize(rate);
            File.WriteAllText(FilePath, json);
        }
    }
}
