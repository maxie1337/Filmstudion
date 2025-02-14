using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.Models.Film;
using API.Models.FilmCopy;
using API.data;
using System.Security.Claims;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/films")]
    public class FilmsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilmsController(AppDbContext context)
        {
            _context = context;
        }

        // Lägger till en ny film
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddFilm([FromBody] CreateFilmDto filmDto)
        {
            if (filmDto == null)
                return BadRequest("Ogiltig data");

            var film = new Film
            {
                Title = filmDto.Title,
                ImageURL = filmDto.ImageURL,
                Director = filmDto.Director,
                Year = filmDto.Year,
                FilmCopies = new List<FilmCopy>()
            };

            // Skapar filmkopior 
            for (int i = 0; i < filmDto.NumberOfCopies; i++)
            {
                film.FilmCopies.Add(new FilmCopy { IsRented = false });
            }

            _context.Films.Add(film);
            _context.SaveChanges();

            return Ok(film);
        }

        // Hämtar en lista med filmer
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetFilms()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var result = _context.Films
                    .Select(f => new {
                        f.FilmId,
                        f.Title,
                        f.Director,
                        f.Year,
                        AvailableCopies = f.FilmCopies.Count(fc => !fc.IsRented)
                    })
                    .ToList();
                return Ok(result);
            }
            else
            {
                var films = _context.Films
                    .Include(f => f.FilmCopies)
                    .ToList()
                    .Select(f => new {
                        f.FilmId,
                        f.Title,
                        f.Director,
                        f.Year,
                        filmCopies = f.FilmCopies.Where(fc => !fc.IsRented).ToList()
                    }).ToList();
                return Ok(films);
            }
        }

        // Hämtar detaljer om en specifik film
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetFilmById(int id)
        {
            var film = _context.Films.Include(f => f.FilmCopies).FirstOrDefault(f => f.FilmId == id);
            if (film == null)
                return NotFound();

            if (!User.Identity.IsAuthenticated)
            {
                var result = new {
                    film.FilmId,
                    film.Title,
                    film.Director,
                    film.Year
                };
                return Ok(result);
            }
            else
            {
                return Ok(film);
            }
        }

        // Uppdaterar en films information
        [Authorize(Roles = "admin")]
        [HttpPatch("{id}")]
        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public IActionResult UpdateFilm(int id, [FromBody] Film updatedFilm)
        {
            if (updatedFilm == null)
                return BadRequest("Ogiltig data");

            var film = _context.Films.Include(f => f.FilmCopies).FirstOrDefault(f => f.FilmId == id);
            if (film == null)
                return NotFound();

            film.Title = updatedFilm.Title;
            film.Director = updatedFilm.Director;
            film.Year = updatedFilm.Year;


            _context.SaveChanges();
            return Ok(film);
        }

        // Filmuthyrning endpoint
        [Authorize(Roles = "filmstudio")]
        [HttpPost("rent")]
        public IActionResult RentFilm([FromQuery] int id, [FromQuery] int studioid)
        {

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int tokenStudioId) || tokenStudioId != studioid)
                return Unauthorized("Otillåtet");

            var film = _context.Films.Include(f => f.FilmCopies).FirstOrDefault(f => f.FilmId == id);
            if (film == null)
                return StatusCode(409, "Film ej hittad");

            var studio = _context.FilmStudios.Include(fs => fs.RentedFilmCopies)
                .FirstOrDefault(fs => fs.FilmStudioId == studioid);
            if (studio == null)
                return StatusCode(409, "Filmstudio ej hittad");

            bool alreadyRented = film.FilmCopies.Any(copy => copy.IsRented && copy.RentedByStudioId == studioid);
            if (alreadyRented)
                return StatusCode(403, "Filmstudio har redan hyrt en kopia");

            var availableCopy = film.FilmCopies.FirstOrDefault(copy => !copy.IsRented);
            if (availableCopy == null)
                return StatusCode(409, "Inga lediga kopior");

            availableCopy.IsRented = true;
            availableCopy.RentedDate = DateTime.Now;
            availableCopy.RentedByStudioId = studioid;
            studio.RentedFilmCopies.Add(availableCopy);

            _context.SaveChanges();
            return Ok(new { message = "Movie rented successfully" });
        }

        // Lämnar tillbaka en hyrd film 
        [Authorize(Roles = "filmstudio")]
        [HttpPost("return")]
        public IActionResult ReturnFilm([FromQuery] int id, [FromQuery] int studioid)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int tokenStudioId) || tokenStudioId != studioid)
                return Unauthorized("Otillåtet");

            var film = _context.Films.Include(f => f.FilmCopies).FirstOrDefault(f => f.FilmId == id);
            if (film == null)
                return StatusCode(409, "Film ej hittad");

            var studio = _context.FilmStudios.Include(fs => fs.RentedFilmCopies)
                .FirstOrDefault(fs => fs.FilmStudioId == studioid);
            if (studio == null)
                return StatusCode(409, "Filmstudio ej hittad");

            var rentedCopy = film.FilmCopies.FirstOrDefault(copy => copy.IsRented && copy.RentedByStudioId == studioid);
            if (rentedCopy == null)
                return StatusCode(409, "Ingen uthyrd kopia hittad");

            rentedCopy.IsRented = false;
            rentedCopy.RentedDate = null;
            rentedCopy.RentedByStudioId = null;
            studio.RentedFilmCopies.Remove(rentedCopy);

            _context.SaveChanges();
            return Ok();
        }
    }
}