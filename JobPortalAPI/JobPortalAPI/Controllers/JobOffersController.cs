using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class JobOffersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public JobOffersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/JobOffers
    [HttpGet]
    public async Task<IActionResult> GetAllJobOffers()
    {
        var jobOffers = await _context.JobOffers.ToListAsync();
        return Ok(jobOffers);
    }

    // GET: api/JobOffers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobOffer(int id)
    {
        var jobOffer = await _context.JobOffers.FindAsync(id);
        if (jobOffer == null) return NotFound("Offre non trouvée.");

        return Ok(jobOffer);
    }

    // POST: api/JobOffers
    [Authorize(Roles = "Admin,Recruiter")]
    [HttpPost]
    public async Task<IActionResult> CreateJobOffer([FromBody] JobOffer model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var jobOffer = new JobOffer
        {
            Title = model.Title,
            Description = model.Description,
            Company = model.Company,
            Location = model.Location,
            SalaryRange = model.SalaryRange,
            ExpiresAt = model.ExpiresAt,
            PublisherId = User.FindFirstValue(ClaimTypes.NameIdentifier) // ID de l'utilisateur connecté
        };

        _context.JobOffers.Add(jobOffer);
        await _context.SaveChangesAsync();

        return Ok("Offre créée avec succès !");
    }


    // PUT: api/JobOffers/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateJobOffer(int id, [FromBody] JobOffer jobOffer)
    {
        if (id != jobOffer.Id) return BadRequest("ID de l'offre invalide.");

        var existingOffer = await _context.JobOffers.FindAsync(id);
        if (existingOffer == null) return NotFound("Offre non trouvée.");

        existingOffer.Title = jobOffer.Title;
        existingOffer.Description = jobOffer.Description;
        existingOffer.Company = jobOffer.Company;
        existingOffer.Location = jobOffer.Location;
        existingOffer.SalaryRange = jobOffer.SalaryRange;
        existingOffer.Status = jobOffer.Status;
        existingOffer.ExpiresAt = jobOffer.ExpiresAt;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/JobOffers/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteJobOffer(int id)
    {
        var jobOffer = await _context.JobOffers.FindAsync(id);
        if (jobOffer == null) return NotFound("Offre non trouvée.");

        _context.JobOffers.Remove(jobOffer);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
