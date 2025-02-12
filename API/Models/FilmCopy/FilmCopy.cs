using System;
using API.Interfaces;

namespace API.Models.FilmCopy;

public class FilmCopy : IFilmCopy
{
    public int CopyId { get; set; }
    public bool IsRented { get; set; }
    public DateTime? RentedDate { get; set; }
        
    // Extra egenskap för att spåra vilken studio som hyr kopian, kolla sen
    public int? RentedByStudioId { get; set; }
}
