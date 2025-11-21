using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10440926_poeparttwo.Models
{
    public class HourlyRateSetting
    {
        [Key]   
        [DatabaseGenerated(DatabaseGeneratedOption.None)]  
        public int Id { get; set; } = 1;   

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
    }
}
