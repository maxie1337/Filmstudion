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

        // POST: /api/filmstudio/register
        // Tillåtet för anonyma användare
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

            var result = new {
                FilmStudioId = newStudio.FilmStudioId,
                Name = newStudio.Name,
                City = newStudio.City
            };

            return Ok(result);
        }

        // GET: /api/filmstudios
        // Tillåter både anonyma och autentiserade anrop
        [HttpGet("filmstudios")]
        [AllowAnonymous]
        public IActionResult GetFilmStudios()
        {
            // Om användaren är autentiserad och har admin-roll returneras fullständig data
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (User.Identity.IsAuthenticated && role == "admin")
            {
                return Ok(_context.FilmStudios.ToList());
            }
            else
            {
                // I övrigt returneras filtrerad data (utan t.ex. City)
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

        // GET: /api/filmstudio/{id}
        // Tillåtet för alla, men visar mer data om användaren är admin eller om filmstudion hämtar sin egen info
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetFilmStudioById(int id)
        {
            var studio = _context.FilmStudios.FirstOrDefault(s => s.FilmStudioId == id);
            if (studio == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            // Om admin – visa all data
            if (User.Identity.IsAuthenticated && role == "admin")
            {
                return Ok(studio);
            }
            // Om filmstudio och tokeninnehållet matchar (här antas att token innehåller filmstudions namn i "sub")
            if (User.Identity.IsAuthenticated && role == "filmstudio")
            {
                var username = User.Identity.Name;
                if (username == studio.Name)
                {
                    return Ok(studio);
                }
            }
            // Annars returneras filtrerad data
            var filtered = new {
                studio.FilmStudioId,
                studio.Name
            };
            return Ok(filtered);
        }
    }
}