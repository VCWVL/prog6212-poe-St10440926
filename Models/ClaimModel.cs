using System.ComponentModel.DataAnnotations;

namespace st10440926_poeparttwo.Models
{
    public class ClaimModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Range(1, 200, ErrorMessage = "Enter valid hours (1–200).")]
        public double HoursWorked { get; set; }

        [Range(100, 1000, ErrorMessage = "Enter hourly rate (100–1000).")]
        public double HourlyRate { get; set; }

        public string? Notes { get; set; }
        public string? FileName { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
