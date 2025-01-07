using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Nom d'utilisateur")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Adresse email")]
    public string Email { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Numéro de téléphone")]
    public string PhoneNumber { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public required string Password { get; set; }  // Required modifier

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmer le mot de passe")]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public required string ConfirmPassword { get; set; } // Required modifier

    [Display(Name = "Je suis un recruteur")]
    public bool IsRecruiter { get; set; }
}
