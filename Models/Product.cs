using ProductManagement.Helpers.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Product Name can't be longer than 100 characters.")]
        public string Name { get; set; }
        [Required]
        [Range(0.01, 10000.0, ErrorMessage = "Price must be between 0.1 and 1000.0.")]
        public decimal? Price { get; set; }

        [StringLength(500, ErrorMessage = "Product description can't be longer than 500 characters.")]
        public string? Description { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity InStock must be a positive number.")]
        [PositiveInteger(ErrorMessage = "Quantity InStock must be a whole number.")]
        public int QuantityInStock { get; set; }

        [Range(0, 100.0, ErrorMessage = "Discount Percentage must be between 0% and 100%")]
        public int DiscountPercentage { get; set; }
        public decimal FinalPrice => Price != null ? Price.Value - (Price.Value * (DiscountPercentage) / 100) : 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
