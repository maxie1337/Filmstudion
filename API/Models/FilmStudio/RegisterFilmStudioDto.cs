using System;
using API.Interfaces;

namespace API.Models.FilmStudio;

public class RegisterFilmStudioDto : IRegisterFilmStudio
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Password { get; set; }
}
