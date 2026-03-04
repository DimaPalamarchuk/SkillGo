using System.ComponentModel.DataAnnotations;
using SkillGo.Data;

namespace SkillGo.Data.Models
{
    public enum WalletTransactionType
    {
        TopUp = 1,
        Debit = 2,
        Refund = 3
    }

    public class WalletTransaction
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        public ApplicationUser? User { get; set; }

        [Range(0, 999999999)]
        public decimal Amount { get; set; }

        public WalletTransactionType Type { get; set; }

        [MaxLength(200)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}