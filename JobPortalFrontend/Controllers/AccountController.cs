using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JobPortalFrontend.Models; // Assurez-vous que les mod�les LoginViewModel et RegisterViewModel sont import�s
using System;
using System.Globalization;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    // Constructeur pour injecter les services n�cessaires
    public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
    }

    // Afficher la vue de connexion
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // G�rer la soumission du formulaire de connexion
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var url = $"{_configuration["ApiBaseUrl"]}/api/Auth/Login";
        var jsonPayload = JsonConvert.SerializeObject(model);
        var response = await _httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            // R�cup�rer la r�ponse JSON
            var result = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
            if (result != null)
            {
                Console.WriteLine($"Token JWT re�u est : {result.Token}");
                Console.WriteLine($"Expiration du token : {result.Expiration}");

                // Enregistrement du token dans un cookie
                Response.Cookies.Append("JWT", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = result.Expiration, // Assurez-vous que l'expiration est correcte
                    SameSite = SameSiteMode.Strict
                });

                // Redirection vers l'URL indiqu�e dans la r�ponse de l'API
                return Redirect(result.RedirectUrl ?? "/Home/Index");
            }
        }
        else
        {
            // Extraire et afficher les erreurs envoy�es par l'API
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"{errorMessage}");
        }

        return View(model);
    }


    // Afficher la vue d'inscription
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // G�rer la soumission du formulaire d'inscription
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var url = $"{_configuration["ApiBaseUrl"]}/api/Auth/Register";
        var jsonPayload = JsonConvert.SerializeObject(model);
        var response = await _httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            // Au lieu de TempData, g�rer le succ�s avec ViewBag ou une r�ponse directe
            ViewBag.SuccessMessage = "Inscription r�ussie ! Vous pouvez maintenant vous connecter.";
            return RedirectToAction("Login");
        }
        else
        {
            // Extraire et afficher les erreurs envoy�es par l'API
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"{errorMessage}");
        }

        return View(model);
    }

    // G�rer la d�connexion
    [HttpPost]
    public IActionResult Logout()
    {
        // Supprimer le cookie JWT
        Response.Cookies.Delete("JWT");
        return RedirectToAction("Login", "Account");
    }
}
