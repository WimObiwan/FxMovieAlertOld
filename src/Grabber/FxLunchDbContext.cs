using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FxMovies.Grabber
{
    /// <summary>
    /// The entity framework context with a Students DbSet 
    /// </summary>
    public class FxLunchDbContext : DbContext
    {
        public FxLunchDbContext(DbContextOptions<FxLunchDbContext> options)
            : base(options)
        { }

        public DbSet<MovieEvent> Students { get; set; }
    }

    /// <summary>
    /// A factory to create an instance of the StudentsContext 
    /// </summary>
    public static class FxLunchDbContextFactory
    {
        public static FxLunchDbContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FxLunchDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            // Ensure that the SQLite database and sechema is created!
            var context = new FxLunchDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }

    /// <summary>
    /// A simple class representing a Student
    /// </summary>
    public class MovieEvent
    {
        public MovieEvent()
        {
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public DateTime StartTime { get; set; }
    }
}
