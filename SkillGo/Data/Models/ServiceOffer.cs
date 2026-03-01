using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillGo.Data.Models
{
    public class ServiceOffer
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        [Required]
        public string OwnerUserId { get; set; } = default!;
        public ApplicationUser OwnerUser { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}