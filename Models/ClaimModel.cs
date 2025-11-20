using System.ComponentModel.DataAnnotations;

namespace st10440926_poeparttwo.Models
{
    public class ClaimModel
    {
        // Unique claim ID
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Name of the lecturer (auto-filled)
        [Required, Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        // Used for filtering — WHO the claim belongs to
        // ⭐ VERY IMPORTANT FOR SECURITY
        public string LecturerUsername { get; set; }

        // Hours worked
        [Range(1, 200, ErrorMessage = "Enter valid hours (1–200).")]
        public double HoursWorked { get; set; }

        // Hourly rate (auto-filled)
        [Range(100, 1000, ErrorMessage = "Enter hourly rate (100–1000).")]
        public double HourlyRate { get; set; }

        // Date submitted
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        // Optional notes
        public string? Notes { get; set; }

        // Attached file name (original)
        public string? FileName { get; set; }

        // Pending / Approved / Rejected
        public string Status { get; set; } = "Pending";
    }
}
