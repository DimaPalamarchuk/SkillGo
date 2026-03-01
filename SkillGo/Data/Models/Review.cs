using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillGo.Data.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string TargetUserId { get; set; } = default!;
        public ApplicationUser TargetUser { get; set; } = default!;

        [Required]
        public string AuthorUserId { get; set; } = default!;
        public ApplicationUser AuthorUser { get; set; } = default!;

        [Range(1, 5)]
        public int Stars { get; set; }

        [NotMapped]
        public int Rating
        {
            get => Stars;
            set => Stars = value;
        }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}