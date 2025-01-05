using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class JobApplicationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public JobApplicationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/JobApplications
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ApplyToJob([FromBody] JobApplication application)
    {
        application.ApplicantId = User.Identity?.Name;

        var job = await _context.JobOffers.FindAsync(application.JobId);
        if (job == null) return NotFound("Offre non trouvée.");

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
    }

    // GET: api/JobApplications/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetApplication(int id)
    {
        var application = await _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.Applicant)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null) return NotFound("Candidature non trouvée.");

        return Ok(application);
    }

    // PUT: api/JobApplications/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] string status)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application == null) return NotFound("Candidature non trouvée.");

        application.Status = status;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
