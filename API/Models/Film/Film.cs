using System;
using API.Interfaces;
using API.data;

namespace API.Models.Film;

   public class Film : IFilm
    {
        public int FilmId { get; set; }
        public string Title { get; set; }
        public string ImageURL {get; set;}
        public string Director { get; set; }
        public int Year { get; set; }
        public List<IFilmCopy> FilmCopies { get; set; } = new List<IFilmCopy>();
    }
