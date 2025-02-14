using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using API.data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/mystudio")]
    [Authorize(Roles = "filmstudio")]
    public class MyStudioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MyStudioController(AppDbContext context)
        {
            _context = context;
        }

        // Hämtar en lista över hyrda filmer av den inloggade filmstudion
        [HttpGet("rentals")]
        public IActionResult GetRentedMovies()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int filmStudioId))
                return Unauthorized("Otillåtet");

            // Hämtar de unika filmId:n som studion har hyrt
            var rentedFilmIds = _context.FilmCopies
                .Where(fc => fc.IsRented && fc.RentedByStudioId == filmStudioId)
                .Select(fc => fc.FilmId)
                .Distinct()
                .ToList();

            // Hämtar filmerna för de hyrda filmId:erna
            var rentedFilms = _context.Films
                .Where(f => rentedFilmIds.Contains(f.FilmId))
                .Select(f => new
                {
                    filmId = f.FilmId,
                    title = f.Title,
                    imageURL = f.ImageURL,
                    director = f.Director,
                    year = f.Year
                })
                .ToList();

            return Ok(rentedFilms);
        }
    }
}
