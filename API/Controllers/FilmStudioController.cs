using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using API.data;
using API.Models.FilmStudio;

namespace API.Controllers
{
    [ApiController]
    [Route("api/filmstudio")]
    public class FilmStudioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilmStudioController(AppDbContext context)
        {
            _context = context;
        }

        // Registrerar en ny filmstudio
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterFilmStudioDto registerDto)
        {
            if (registerDto == null || string.IsNullOrEmpty(registerDto.Name) ||
                string.IsNullOrEmpty(registerDto.City) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Ogiltigt Namn, lösenord eller stad");
            }

            var newStudio = new FilmStudio
            {
                Name = registerDto.Name,
                City = registerDto.City,
                Password = registerDto.Password
            };

            _context.FilmStudios.Add(newStudio);
            _context.SaveChanges();

            var result = new
            {
                FilmStudioId = newStudio.FilmStudioId,
                Name = newStudio.Name,
                City = newStudio.City
            };

            return Ok(result);
        }

        // Hämtar lista över filmstudios (admin ser all information, andra ser filtrerad info)
        [HttpGet("filmstudios")]
        [AllowAnonymous]
        public IActionResult GetFilmStudios()
        {
            // Om användaren är admin all info
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (User.Identity.IsAuthenticated && role == "admin")
            {
                return Ok(_context.FilmStudios.ToList());
            }
            else
            {
                // Annars returneras filtrerad info
                var result = _context.FilmStudios
                    .Select(fs => new
                    {
                        fs.FilmStudioId,
                        fs.Name
                    })
                    .ToList();
                return Ok(result);
            }
        }

        // Hämtar en specifik filmstudio, admin och den filmstudion man hämtar ser all info
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetFilmStudioById(int id)
        {
            var studio = _context.FilmStudios.FirstOrDefault(s => s.FilmStudioId == id);
            if (studio == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            // Om admin hämtar info, visa allt
            if (User.Identity.IsAuthenticated && role == "admin")
            {
                return Ok(studio);
            }
            // Om filmstudion hämtar sin egen info, visa allt
            if (User.Identity.IsAuthenticated && role == "filmstudio")
            {
                var username = User.Identity.Name;
                if (username == studio.Name)
                {
                    return Ok(studio);
                }
            }
            // Annars skicka tillbaka denna info
            var filtered = new
            {
                studio.FilmStudioId,
                studio.Name
            };
            return Ok(filtered);
        }
    }
}