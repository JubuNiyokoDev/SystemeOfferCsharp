using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<CustomUser> _userManager;

    public UsersController(UserManager<CustomUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/Users
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users;
        return Ok(users);
    }

    // POST: api/Users/Ban/{id}
    [HttpPost("Ban/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BanUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("Utilisateur non trouvé.");

        user.IsBanned = true;
        await _userManager.UpdateAsync(user);

        return Ok("Utilisateur banni avec succès.");
    }

    // POST: api/Users/Unban/{id}
    [HttpPost("Unban/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnbanUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("Utilisateur non trouvé.");

        user.IsBanned = false;
        await _userManager.UpdateAsync(user);

        return Ok("Utilisateur débanni avec succès.");
    }
}
