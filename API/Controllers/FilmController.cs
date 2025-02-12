using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.data;

namespace API.Controllers
{

[ApiController]
[Route("api/films")]

public class FilmController : ControllerBase
{
    private readonly AppDbContext _context;
    public FilmController(AppDbContext context) { _context = context; }

    [HttpGet]
    public IActionResult GetFilms() => Ok(_context.Films.ToList());
} 

}
