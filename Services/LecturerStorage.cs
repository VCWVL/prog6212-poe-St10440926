using System.Text.Json;
using st10440926_poeparttwo.Models;

namespace st10440926_poeparttwo.Services
{
    public static class LecturerStorage
    {
        private static readonly string FilePath = Path.Combine("App_Data", "lecturers.json");

        public static List<LecturerProfile> LoadLecturers()
        {
            if (!File.Exists(FilePath))
                return new List<LecturerProfile>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<LecturerProfile>>(json)
                   ?? new List<LecturerProfile>();
        }

        public static void SaveLecturers(List<LecturerProfile> lecturers)
        {
            var json = JsonSerializer.Serialize(lecturers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void AddLecturer(LecturerProfile lecturer)
        {
            var list = LoadLecturers();
            list.Add(lecturer);
            SaveLecturers(list);
        }

        public static LecturerProfile GetLecturer(string username)
        {
            return LoadLecturers()
                   .FirstOrDefault(x => x.Username == username);
        }
    }
}
