using Microsoft.AspNetCore.Mvc;
using JobPortalFrontend.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace JobPortalFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            // Vérification si l'utilisateur est authentifié via le cookie JWT
            var jwtToken = Request.Cookies["JWT"];

            if (!string.IsNullOrEmpty(jwtToken))
            {
                // Décodez le JWT pour extraire les informations de l'utilisateur
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var userName = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "name")?.Value;
                    return Content($"Bienvenue, {userName} !");
                }
            }

            // Si le JWT est absent ou invalide
            return Content("Utilisateur non authentifié");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
