﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxMovies.Grabber
{
    public class FxLunchDB
    {
        string connectionString;

        public FxLunchDB(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void RemoveForDate(DateTime dateTime)
        {
            using (var context = FxLunchDbContextFactory.Create(connectionString))
            {
                var set = context.Set<MovieEvent>();
                set.RemoveRange(set.Where(x => x.StartTime.Date == dateTime.Date));
                context.SaveChanges();
            }
        }

        public void Save(Movie movie)
        {
            var movieEvent = new MovieEvent()
            {
                Title = movie.Title,
                Year = movie.Year,
                StartTime = movie.StartTime,
            };

            using (var context = FxLunchDbContextFactory.Create(connectionString))
            {
                context.Add(movieEvent);
                context.SaveChanges();
            }

            //Console.WriteLine($"Student was saved in the database with id: {movieEvent.Id}");
        }
    }
}
