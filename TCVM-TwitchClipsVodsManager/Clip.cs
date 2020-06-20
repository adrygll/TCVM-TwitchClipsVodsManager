using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCVM_TwitchClipsVodsManager
{
    class Clip
    {
        private string id;
        private string slug;
        private string game;
        private string title;
        private int views;
        private int duration;
        private DateTime date;
        private string[] thumbnails;

        public Clip(string id, string slug, string game, string title, int views, int duration, DateTime date, string[] thumbnails)
        {
            this.Id = id;
            this.Slug = slug;
            this.Game = game;
            this.Title = title;
            this.Views = views;
            this.Duration = duration;
            this.Date = date;
            this.Thumbnails = thumbnails;
        }

        public string Slug { get => slug; set => slug = value; }
        public string Game { get => game; set => game = value; }
        public string Title { get => title; set => title = value; }
        public int Views { get => views; set => views = value; }
        public int Duration { get => duration; set => duration = value; }
        public DateTime Date { get => date; set => date = value; }
        public string[] Thumbnails { get => thumbnails; set => thumbnails = value; }
        public string Id { get => id; set => id = value; }
    }
}
