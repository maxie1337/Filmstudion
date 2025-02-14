using System;
using API.Interfaces;

namespace API.Models.FilmCopy;

public class FilmCopy : IFilmCopy
{
    public int FilmCopyId { get; set; }
    public int FilmId { get; set; }
    public bool IsRented { get; set; }
    public DateTime? RentedDate { get; set; }
    public int? RentedByStudioId { get; set; }
}
