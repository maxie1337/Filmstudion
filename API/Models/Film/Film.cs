using System;
using API.Interfaces;
using ConcreteFilmCopy = API.Models.FilmCopy.FilmCopy;

namespace API.Models.Film;

   public class Film : IFilm
    {
        public int FilmId { get; set; }
        public string Title { get; set; }
        public string? ImageURL {get; set;}
        public string Director { get; set; }
        public int Year { get; set; }
        public List<ConcreteFilmCopy> FilmCopies { get; set; } = new List<ConcreteFilmCopy>();
    }
