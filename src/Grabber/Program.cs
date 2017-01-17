using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace FxMovies.Grabber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Enable to app to read json setting files
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            // Get the connection string
            string connectionString = configuration.GetConnectionString("FxMoviesDb");

            DateTime date = DateTime.Now.Date.AddDays(1);
            var movies = HumoGrabber.GetGuide(date).Result;

            var fxLunchDB = new FxLunchDB(connectionString);

            Console.WriteLine(date.ToString());
            foreach (var movie in movies)
            {
                fxLunchDB.Save(movie);

                Console.WriteLine("{0} {1} {2} {4} {5}", movie.Channel.Name, movie.Title, movie.Year, movie.Channel.Name, movie.StartTime, movie.EndTime);
            }

        }

    }
}
