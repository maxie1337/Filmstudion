using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.Models.Film;
using API.Models.FilmCopy;
using API.data;
using System.Security.Claims;
using System;

namespace MyFilmApi.Controllers
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

        // POST: /api/films
        // Endast admin får lägga till filmer
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddFilm([FromBody] CreateFilmDto filmDto)
        {
            if (filmDto == null)
                return BadRequest("Ogiltig data");

            var film = new Film
            {
                Title = filmDto.Title,
                Director = filmDto.Director,
                Year = filmDto.Year
            };

            // Skapa filmkopior enligt angivet antal
            for (int i = 0; i < filmDto.NumberOfCopies; i++)
            {
                film.FilmCopies.Add(new FilmCopy { IsRented = false });
            }

            _context.Films.Add(film);
            _context.SaveChanges();

            return Ok(film);
        }

        // GET: /api/films
        // Tillgänglig för alla
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
                                         f.Year
                                     })
                                     .ToList();
                return Ok(result);
            }
            else
            {
                return Ok(_context.Films.Include(f => f.FilmCopies).ToList());
            }
        }

        // GET: /api/films/{id}
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

        // PATCH/PUT/POST: /api/films/{id} – Endast admin får uppdatera filmdata
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
            // Om du vill uppdatera kopiorna kan du lägga till logik här

            _context.SaveChanges();
            return Ok(film);
        }

        // POST: /api/films/rent?id={id}&studioid={studioid}
        // Endast filmstudio får hyra filmer
        [Authorize(Roles = "filmstudio")]
        [HttpPost("rent")]
        public IActionResult RentFilm([FromQuery] int id, [FromQuery] int studioid)
        {
            // Hämta filmstudions id från token (claim "UserId")
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

            bool alreadyRented = film.FilmCopies.Any(copy => copy.IsRented && RentedByStudioId == studioid);
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
            return Ok();
        }

        // POST: /api/films/return?id={id}&studioid={studioid}
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
