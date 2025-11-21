using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10440926_poeparttwo.Models
{
    public class UserModel
    {
        [Key]   // ⭐ Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }   // HR, Lecturer, Coordinator, Manager
    }
}
