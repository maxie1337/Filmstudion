using System;
using API.Interfaces;

namespace API.Models.Film;

public class CreateFilmDto : ICreateFilm
{
     public string Title { get; set; }
     public string Director { get; set; }
     public string? ImageURL{get; set;}
     public int Year { get; set; }
     public int NumberOfCopies { get; set; }

}
