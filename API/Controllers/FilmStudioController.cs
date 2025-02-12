using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.data;

namespace API.Controllers
{

[ApiController]
[Route("api/filmstudio")]

public class FilmStudioController : ControllerBase
{
    private readonly AppDbContext _context;
    public FilmStudioController(AppDbContext context) { _context = context; }

    [HttpGet]
    public IActionResult GetFilmStudios() => Ok(_context.FilmStudios.ToList());
}

}
