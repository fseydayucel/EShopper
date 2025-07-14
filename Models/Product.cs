using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectMvc.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }  // e.g., "Colorful Stylish Shirt"

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }  // e.g., 150.00

        public string Description { get; set; }  // Full product description

        public List<string> AvailableSizes { get; set; }  // XS, S, M, L, XL

        public List<string> AvailableColors { get; set; }  // Black, White, Red...

        public string ImageUrl { get; set; }  // img/product-1.jpg, etc.

        public int StockQuantity { get; set; }  // for inventory, optional

        [Range(0, 5)]
        public double Rating { get; set; }  // e.g., 4.5

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
