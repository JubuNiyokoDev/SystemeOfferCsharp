using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsRecruiter { get; set; } 
}
