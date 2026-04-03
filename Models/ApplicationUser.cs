using Microsoft.AspNetCore.Identity;

namespace Collabo_app.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? avatarUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsOnline { get; set; } = false;
        public string? Role { get; set; }
    }
}
