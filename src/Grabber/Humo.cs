﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace FxMovies.Grabber
{
    public static class HumoGrabber
    {
        #region JSonModel
        private class Resized_Url
        {
            public string large { get; set; }
            public string medium { get; set; }
            public string small { get; set; }
        }

        [DebuggerDisplay("link_type = {link_type}")]
        private class Media
        {
            public int id { get; set; }
            public string link_type { get; set; }
            public string media_type { get; set; }
            public string caption { get; set; }
            public Resized_Url resized_urls { get; set; }
        }

        private class EventProperties
        {
            public int eventduration { get; set; }
            public int hd { get; set; }
            public int live { get; set; }
            public int part_of_series { get; set; }
            public int series_id { get; set; }
        }

        [DebuggerDisplay("title = {title}")]
        private class EventProgram
        {
            public int id { get; set; }
            public int episodenumber { get; set; }
            public string title { get; set; }
            public int year { get; set; }
            public string description { get; set; }
            public string content_short { get; set; }
            public string content_long { get; set; }
            public List<string> genres { get; set; }
            public List<Media> media { get; set; }
        }

        [DebuggerDisplay("title = {program.title}")]
        private class Event
        {
            public int id { get; set; }
            public string url { get; set; }
            public int event_id { get; set; }
            public int starttime { get; set; }
            public int endtime { get; set; }
            public List<string> labels { get; set; }
            public EventProperties properties { get; set; }
            public EventProgram program { get; set; }
        }

        [DebuggerDisplay("display_name = {display_name}")]
        private class BroadCasters
        {
            public int id { get; set; }
            public string code { get; set; }
            public string display_name { get; set; }
            public List<Media> media { get; set; }
            public List<Event> events { get; set; }
        }

        private class Humo
        {
            public string platform { get; set; }
            public string date { get; set; }
            public List<BroadCasters> broadcasters { get; set; }
        }
        #endregion

        public static async Task<IList<Movie>> GetGuide(DateTime date)
        {
            string dateYMD = date.ToString("yyyy-MM-dd");
            string url = string.Format("http://www.humo.be/api/epg/humosite/schedule/main/{0}/full", dateYMD);

            var request = WebRequest.CreateHttp(url);
            using (var response = await request.GetResponseAsync())
            {
                using (var textStream = new StreamReader(response.GetResponseStream()))
                {
                    string json = await textStream.ReadToEndAsync();

                    var humo = JsonConvert.DeserializeObject<Humo>(json);

                    FilterMovies(humo);

                    return MovieAdapter(humo);
                }
            }
        }

        private static void FilterMovies(Humo humo)
        {
            foreach (var broadcaster in humo.broadcasters)
            {
                broadcaster.events.RemoveAll(e => e.labels == null || !e.labels.Contains("film"));
            }

            humo.broadcasters.RemoveAll(b => (b.events.Count == 0));
        }

        private static IList<Movie> MovieAdapter(Humo humo)
        {
            var movies = new List<Movie>();
            foreach (var broadcaster in humo.broadcasters)
            {
                var channel = new Channel()
                {
                    Name = broadcaster.display_name,
                };

                foreach (var evnt in broadcaster.events)
                {
                    var movie = new Movie()
                    {
                        Channel = channel,
                        Title = evnt.program.title,
                        Year = evnt.program.year,
                        StartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(evnt.starttime).ToLocalTime(),
                        EndTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(evnt.endtime).ToLocalTime(),
                    };

                    movies.Add(movie);
                }
            }

            return movies;
        }
    }
}
