using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergyConnect.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }

        [Required]
        public DateOnly ProdDate { get; set; }

        [Display(Name = "Image URL")]
        public string imageURL { get; set; }

        [NotMapped]
        [Display(Name = "Product Image")]
        public IFormFile? ImageFile { get; set; }

        public string UserId { get; set; }
    }
}
