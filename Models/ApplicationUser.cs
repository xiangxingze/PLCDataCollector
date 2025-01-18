using Microsoft.AspNetCore.Identity;

namespace PLCDataCollector.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string DisplayName { get; set; }
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Administrator,
        Operator,
        Viewer
    }
} 