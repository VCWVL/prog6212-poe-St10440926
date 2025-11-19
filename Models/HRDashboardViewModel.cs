using System.Collections.Generic;

namespace st10440926_poeparttwo.Models
{
    public class HRDashboardViewModel
    {
        public List<UserModel> Users { get; set; }
        public List<LecturerProfile> LecturerProfiles { get; set; }
        public decimal StandardRate { get; set; }
    }
}
