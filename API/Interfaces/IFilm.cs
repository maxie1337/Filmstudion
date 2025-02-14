using System;
using API.Models.FilmCopy;

namespace API.Interfaces;

 public interface IFilm
    {
        int FilmId { get; set; }
        string Title { get; set; }
        string? ImageURL {get; set;}
        string Director { get; set; }
        int Year { get; set; }
        List<FilmCopy> FilmCopies { get; set; }
        
    }
