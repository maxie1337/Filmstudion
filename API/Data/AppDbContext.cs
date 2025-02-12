using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using API.Models.Film;
using API.Models.FilmStudio;

namespace API.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        this.Database.EnsureCreated();

    }

    public DbSet<Film> Films { get; set; }
    public DbSet<FilmStudio> FilmStudios { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

    }
}