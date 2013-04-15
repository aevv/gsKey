using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gsKey.APIProxy
{
    class Status
    {
        private long life;

        public long Life
        {
            get { return life; }
            set { life = value; }
        }
        private bool playing;

        public bool Playing
        {
            get { return playing; }
            set { playing = value; if (value) state = "playing"; else state = "not"; }
        }
        private string state;

        public string State
        {
            get { return state; }
            set { state = value; if (value.Equals("playing")) playing = true; else playing = false; }
        }
        private string songName;

        public string SongName
        {
            get { return songName; }
            set { songName = value; }
        }

        private string artistName;

        public string ArtistName
        {
            get { return artistName; }
            set { artistName = value; }
        }
        private string albumName;

        public string AlbumName
        {
            get { return albumName; }
            set { albumName = value; }
        }
        private double duration;

        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        private double position;

        public double Position
        {
            get { return position; }
            set { position = value; }
        }
        private string artURL;

        public string ArtURL
        {
            get { return artURL; }
            set { artURL = value; }
        }
        private int trackNo;

        public int TrackNo
        {
            get { return trackNo; }
            set { trackNo = value; }
        }
    }
}
