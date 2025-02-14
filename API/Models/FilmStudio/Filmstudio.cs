using System;
using API.Interfaces;
using ConcreteFilmCopy = API.Models.FilmCopy.FilmCopy;

namespace API.Models.FilmStudio;

public class FilmStudio : IFilmStudio
{
    public int FilmStudioId { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public List<ConcreteFilmCopy> RentedFilmCopies { get; set; } = new List<ConcreteFilmCopy>();
    public string Password { get; set; }
}
