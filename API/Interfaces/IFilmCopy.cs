using System;

namespace API.Interfaces;

public interface IFilmCopy
{
    int CopyId { get; set; }
    bool IsRented { get; set; }
    DateTime? RentedDate { get; set; }
}
