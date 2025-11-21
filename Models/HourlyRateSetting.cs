using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10440926_poeparttwo.Models
{
    public class HourlyRateSetting
    {
        [Key]   // ⭐ Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]  // ⭐ Prevent auto-increment
        public int Id { get; set; } = 1;   // Always 1 – only one row

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
    }
}
