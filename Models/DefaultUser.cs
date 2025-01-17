using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnect.Models
{
    public class DefaultUser: IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public string Address { get; set; }

        [PersonalData]
        public string City { get; set; }

        public DateTime UserCreationDate { get; set; } = DateTime.Now;
    }
}
