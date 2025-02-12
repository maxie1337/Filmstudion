using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.data;
using API.Models.FilmStudio;
using System.Text;
using System.Security.Cryptography;


namespace API.Controllers
{

[ApiController]
[Route("api/filmstudios")]
public class FilmStudioController : ControllerBase
{
    private readonly AppDbContext _context;
    public FilmStudioController(AppDbContext context) { _context = context; }

    [HttpGet]
    public IActionResult GetFilmStudios() => Ok(_context.FilmStudios.Select(fs => new { fs.FilmStudioId, fs.Name, fs.City, fs.Email }).ToList());

    [HttpPost("register")]
    public IActionResult RegisterFilmStudio([FromBody] RegisterFilmStudioDto newStudio)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var filmStudio = new FilmStudio
        {
            Name = newStudio.Name,
            City = newStudio.City,
            Email = newStudio.Email,
            PasswordHash = HashPassword(newStudio.Password)
        };

        _context.FilmStudios.Add(filmStudio);
        _context.SaveChanges();

        return Ok(new { filmStudio.FilmStudioId, filmStudio.Name, filmStudio.City, filmStudio.Email });
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
  }
}