using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_app.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required ...")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required ...")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "min: 2, max: 20")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Price is required ...")]
        [Range(0.01, 100000.00, ErrorMessage = "min: 0.01, max: 100000.00")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Description is required ...")]
        [StringLength(1024, MinimumLength = 2, ErrorMessage = "min: 2, max: 1024")]
        public string? Description { get; set; }
        //Image product
        [NotMapped]
        public IFormFile? ImageData { get; set; }
        //Format image
        public string? ImageType { get; set; }
        public byte[]? ImageFile { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Price: {Price}, Description: {Description}";
        }
    }
}
