using System.Text.Json;
using st10440926_poeparttwo.Models;

namespace st10440926_poeparttwo.Services
{
    public static class UserStorage
    {
        private static readonly string FilePath = Path.Combine("App_Data", "users.json");

        public static List<UserModel> LoadUsers()
        {
            if (!File.Exists(FilePath))
                return new List<UserModel>();

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<UserModel>>(json)
                   ?? new List<UserModel>();
        }

        public static void SaveUsers(List<UserModel> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void AddUser(UserModel user)
        {
            var users = LoadUsers();
            users.Add(user);
            SaveUsers(users);
        }
    }
}
