using System;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectMvc.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }  // Foreign key to Product

        [Required]
        [StringLength(100)]
        public string ReviewerName { get; set; }  // e.g., John Doe

        [Required]
        [EmailAddress]
        public string ReviewerEmail { get; set; }

        [Required]
        [Range(0, 5)]
        public double Rating { get; set; }  // e.g., 4.5 stars

        [Required]
        public string Message { get; set; }  // The review text

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // Optional: Navigation property
        public Product Product { get; set; }
    }
}
