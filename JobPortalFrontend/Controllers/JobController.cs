using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class JobController : Controller
{
    private readonly HttpClient _httpClient;

    public JobController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Afficher toutes les offres d'emploi
    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("http://localhost:5269/api/joboffers");

        if (!response.IsSuccessStatusCode)
        {
            // G�rer les erreurs de l'API
            ViewData["Error"] = "Impossible de charger les offres d'emploi.";
            return View(new List<JobOffer>()); // Passez une liste vide pour �viter l'erreur
        }

        var content = await response.Content.ReadAsStringAsync();
        var jobOffers = JsonConvert.DeserializeObject<List<JobOffer>>(content);

        return View(jobOffers ?? new List<JobOffer>()); // Assurez-vous de passer une liste vide si null
    }



    // Afficher le d�tail d'une offre d'emploi
    public async Task<IActionResult> Details(int id)
    {
        var response = await _httpClient.GetAsync($"http://localhost:5269/api/joboffers/{id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var jobOffer = JsonConvert.DeserializeObject<JobOffer>(content);

        return View(jobOffer);
    }

    // Cr�er une nouvelle offre d'emploi
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(JobOffer jobOffer)
    {
        if (ModelState.IsValid)
        {
            var json = JsonConvert.SerializeObject(jobOffer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5269/api/joboffers", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
        }
        return View(jobOffer);
    }
}
