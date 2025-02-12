using System;

namespace API.Interfaces;

public interface ICreateFilm
    {
        string Title { get; set; }
        string Director { get; set; }
        string ImageURL {get; set;}
        int Year { get; set; }
        int NumberOfCopies { get; set; }
    }
