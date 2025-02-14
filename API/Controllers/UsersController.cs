using Microsoft.AspNetCore.Mvc;
using System.Linq;
using API.Models.User;
using API.data;
using API.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        public UsersController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // Post för adminregistrering
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserRegisterDto registerDto)
        {
            if (registerDto == null || string.IsNullOrEmpty(registerDto.Username) ||
                string.IsNullOrEmpty(registerDto.Password))
                return BadRequest("Ogiltig data");

            if (registerDto.IsAdmin)
            {
                var newUser = new User
                {
                    Username = registerDto.Username,
                    Password = registerDto.Password,
                    Role = "admin"
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                var token = _jwtService.GenerateToken(newUser);
                var result = new
                {
                    newUser.UserId,
                    newUser.Username,
                    newUser.Role,
                    Token = token
                };
                return Ok(result);
            }

            return BadRequest("Endast adminregistrering sker via denna endpoint");
        }

        // Autentiserar användare (admin eller filmstudio) och returnerar JWT om autentiseringen är lyckad
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] UserAuthenticateDto authDto)
        {
            if (authDto == null || string.IsNullOrEmpty(authDto.Username) ||
                string.IsNullOrEmpty(authDto.Password))
                return BadRequest("Ogiltig data");

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == authDto.Username && u.Password == authDto.Password);
            if (user != null)
            {
                var token = _jwtService.GenerateToken(user);
                return Ok(new
                {
                    user.UserId,
                    user.Username,
                    user.Role,
                    Token = token
                });
            }
            
            var studio = _context.FilmStudios.FirstOrDefault(fs =>
                fs.Name == authDto.Username && fs.Password == authDto.Password);
            if (studio != null)
            {
                var pseudoUser = new User
                {
                    UserId = studio.FilmStudioId,
                    Username = studio.Name,
                    Role = "filmstudio"
                };

                var token = _jwtService.GenerateToken(pseudoUser);
                return Ok(new
                {
                    UserId = studio.FilmStudioId,
                    Username = studio.Name,
                    Role = "filmstudio",
                    FilmStudioId = studio.FilmStudioId,
                    FilmStudio = new
                    {
                        studio.FilmStudioId,
                        studio.Name,
                        studio.City
                    },
                    Token = token
                });
            }

            return Unauthorized();
        }
    }
}
