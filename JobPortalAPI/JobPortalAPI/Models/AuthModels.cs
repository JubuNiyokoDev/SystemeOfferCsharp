using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
    public string Password { get; set; }
}

public class LoginModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
