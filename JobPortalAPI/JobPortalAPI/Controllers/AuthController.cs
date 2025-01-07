using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<CustomUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<CustomUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Enregistre un nouvel utilisateur.
    /// </summary>
    /// <param name="model">Données d'inscription.</param>
    /// <returns>Statut de l'enregistrement.</returns>
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Vérifier si l'utilisateur ou l'email existe déjà
        var existingUser = await _userManager.FindByNameAsync(model.Username);
        if (existingUser != null)
        {
            return Conflict("Un utilisateur avec ce nom d'utilisateur existe déjà.");
        }

        var existingEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingEmail != null)
        {
            return Conflict("Un utilisateur avec cet email existe déjà.");
        }

        // Créer un nouvel utilisateur
        var user = new CustomUser
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber // Ajout du numéro de téléphone
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // Ajouter un rôle par défaut (User)
        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "Utilisateur enregistré avec succès !" });
    }

    [HttpGet("CheckAuth")]
    [Authorize] // Vérifie si le JWT est valide
    public IActionResult CheckAuth()
    {
        var userName = User.Identity?.Name;
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        return Ok(new { isAuthenticated, userName });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        Console.WriteLine($"Tentative de connexion : {JsonConvert.SerializeObject(model)}");
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Échec de la validation du modèle.");
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            Console.WriteLine("Échec de l'authentification. Nom d'utilisateur ou mot de passe incorrect.");
            return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
        }

        // Générer le JWT
        var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var expirationTime = DateTime.UtcNow.AddHours(3); // expiration par défaut de 3 heures

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: expirationTime,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"JWT généré pour {user.UserName} : {tokenString}");

        // Définir le cookie JWT
        Response.Cookies.Append("JWT", tokenString, new CookieOptions
        {
            HttpOnly = true, // Sécurise le cookie
            SameSite = SameSiteMode.Lax, // Permet l'accès en local
            Secure = false, // Devrait être true en production avec HTTPS
            Expires = token.ValidTo
        });

        // Retourner une réponse avec succès et inclure la date d'expiration sous forme de chaîne
        return Ok(new
        {
            message = "Connexion réussie !",
            token = tokenString,
            expiration = token.ValidTo,
            redirectUrl = "/Home/Index" // URL à suivre côté client
        });

    }

}
