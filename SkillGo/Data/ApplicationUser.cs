using Microsoft.AspNetCore.Identity;

namespace SkillGo.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }

        public decimal Balance { get; set; } = 0m;
    }
}