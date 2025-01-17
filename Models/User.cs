using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Models
{
    public class User
    {
   
        public int Id { get; set; }

        public string FirstName { get; set; }


        public string LastName { get; set; }
     

        public string Email { get; set; }
     

        public string PhoneNumber { get; set; }
     

        public string PhysicalAddress { get; set; }

        public string City { get; set; }    
    }
}
