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

    }
}