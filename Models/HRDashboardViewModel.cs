using System.Collections.Generic;

namespace st10440926_poeparttwo.Models
{
    public class HRDashboardViewModel
    {
        // Standard hourly rate for all lecturers
        public decimal StandardRate { get; set; }

        // All lecturer profiles (used in "Lecturer Profiles" table)
        public List<LecturerProfile> Lecturers { get; set; }

        // All claims from the system (used to check report availability)
        public List<ClaimModel> Claims { get; set; }

        // All user accounts (only needed if HR creates Users)
        public List<UserModel> Users { get; set; }
    }
}
