namespace st10440926_poeparttwo.Models
{
    public class UserModel
    {
        public string Username { get; set; }   // Required for login
        public string Password { get; set; }   // Required for login
        public string Role { get; set; }       // Lecturer, Coordinator, Manager, HR
    }
}
