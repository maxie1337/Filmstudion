using System;

namespace API.Interfaces;

public interface IFilm
{
    int FilmId { get; set; }
    string Title { get; set; }
    string Genre { get; set; }
}
