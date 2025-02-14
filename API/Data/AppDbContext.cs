using System;
using Microsoft.EntityFrameworkCore;
using API.Models.Film;
using API.Models.FilmStudio;
using API.Models.User;
using API.Models.FilmCopy;

namespace API.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        this.Database.EnsureCreated();

    }

    public DbSet<Film> Films { get; set; }
    public DbSet<FilmStudio> FilmStudios { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<FilmCopy> FilmCopies { get; set; }
    
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Film>()
        .HasMany(f => f.FilmCopies)
        .WithOne()
        .HasForeignKey(fc => fc.FilmId);

    // Skapar 10 filmer
    modelBuilder.Entity<Film>().HasData(
        new Film { FilmId = 1, Title = "Movie 1", ImageURL = "https://placehold.co/100", Director = "Director 1", Year = 2001 },
        new Film { FilmId = 2, Title = "Movie 2", ImageURL = "https://placehold.co/100", Director = "Director 2", Year = 2002 },
        new Film { FilmId = 3, Title = "Movie 3", ImageURL = "https://placehold.co/100", Director = "Director 3", Year = 2003 },
        new Film { FilmId = 4, Title = "Movie 4", ImageURL = "https://placehold.co/100", Director = "Director 4", Year = 2004 },
        new Film { FilmId = 5, Title = "Movie 5", ImageURL = "https://placehold.co/100", Director = "Director 5", Year = 2005 },
        new Film { FilmId = 6, Title = "Movie 6", ImageURL = "https://placehold.co/100", Director = "Director 6", Year = 2006 },
        new Film { FilmId = 7, Title = "Movie 7", ImageURL = "https://placehold.co/100", Director = "Director 7", Year = 2007 },
        new Film { FilmId = 8, Title = "Movie 8", ImageURL = "https://placehold.co/100", Director = "Director 8", Year = 2008 },
        new Film { FilmId = 9, Title = "Movie 9", ImageURL = "https://placehold.co/100", Director = "Director 9", Year = 2009 },
        new Film { FilmId = 10, Title = "Movie 10", ImageURL = "https://placehold.co/100", Director = "Director 10", Year = 2010 }
    );

    // Skapar 5 filmkopior till varje film 
    var filmCopies = new List<FilmCopy>();
    int copyId = 1;
    for (int filmId = 1; filmId <= 10; filmId++)
    {
        for (int i = 0; i < 5; i++)
        {
            filmCopies.Add(new FilmCopy
            {
                FilmCopyId = copyId++,
                FilmId = filmId,
                IsRented = false
            });
        }
    }
    modelBuilder.Entity<FilmCopy>().HasData(filmCopies);
}
}