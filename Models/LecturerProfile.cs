namespace st10440926_poeparttwo.Models
{
    public class LecturerProfile
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }   // <<--- NEW REQUIRED FIELD
        public decimal HourlyRate { get; set; }
    }
}
