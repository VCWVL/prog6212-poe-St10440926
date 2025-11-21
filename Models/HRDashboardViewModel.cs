using System.Collections.Generic;

namespace st10440926_poeparttwo.Models
{
    public class HRDashboardViewModel
    {
        // Standard hourly rate for all lecturers
        public decimal StandardRate { get; set; }

        // All lecturer profiles 
        public List<LecturerProfile> Lecturers { get; set; }

        // All claims 
        public List<ClaimModel> Claims { get; set; }

        // All user accounts 
        public List<UserModel> Users { get; set; }
    }
}
