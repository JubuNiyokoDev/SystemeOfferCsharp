using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

public class CustomUser : IdentityUser
{
    [Phone]
    [StringLength(15)]
    public string PhoneNumber { get; set; }

    public bool IsBanned { get; set; } = false;
    public bool IsRecruiter { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
