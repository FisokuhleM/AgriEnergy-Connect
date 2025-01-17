using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class RegistrationRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string PhysicalAddress { get; set; }
        [Required]
        public string City { get; set; }
    }
}
