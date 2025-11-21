using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10440926_poeparttwo.Models
{
    public class LecturerProfile
    {
        [Key]  
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Username { get; set; }  

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        
        [ForeignKey("Username")]
        public UserModel User { get; set; }
    }
}
