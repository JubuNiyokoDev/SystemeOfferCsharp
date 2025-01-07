using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

public class CustomUser : IdentityUser
{
    [Phone]
    [Required]
    [StringLength(15)]
    public override string PhoneNumber { get; set; } = string.Empty;

    public bool IsBanned { get; set; } = false;
    public bool IsRecruiter { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
