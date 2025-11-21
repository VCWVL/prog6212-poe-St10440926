using System.Collections.Generic;

namespace st10440926_poeparttwo.Models
{
    public class HRDashboardViewModel
    {
        // Standard hourly rate for all lecturers
        public decimal StandardRate { get; set; }

        // All lecturer profiles (SQL table: LecturerProfiles)
        public List<LecturerProfile> Lecturers { get; set; }

        // All claims (SQL table: Claims)
        public List<ClaimModel> Claims { get; set; }

        // All user accounts (SQL table: Users)
        public List<UserModel> Users { get; set; }
    }
}
