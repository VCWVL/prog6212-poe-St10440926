using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10440926_poeparttwo.Models
{
    public class ClaimModel
    {
        [Key]   
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string LecturerName { get; set; }

        [Required]
        public string LecturerUsername { get; set; } 

        [Range(1, 200)]
        public double HoursWorked { get; set; }

        [Range(100, 1000)]
        public double HourlyRate { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        public string? FileName { get; set; }

        public string Status { get; set; } = "Pending";

       

    }
}
