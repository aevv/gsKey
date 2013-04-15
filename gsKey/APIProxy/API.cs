using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.IO;

namespace gsKey.APIProxy
{
    class API
    {
        private int portNumber = 12000;
        int volume = 50;

        public int PortNumber
        {
            get { return portNumber; }
            set
            {
                if (portNumber < 0 || portNumber > 65535)
                {
                }
                else
                    portNumber = value;
            }
        }

        public bool togglePlay()
        {
            return togglePlay();
        }
        public bool togglePlay(ref Status status)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadData("http://localhost:" + portNumber + "/play");
                    long life = status.Life;
                    string title = status.SongName;
                    Status now = getCurrentStatus();
                    if (title.Equals(now.SongName))
                        now.Life = life;
                    status = now;
                    if (now.Playing)
                    {
                        status.Playing = true;
                    }
                    else
                    {
                        status.Playing = false;
                    }
                    status.Position = now.Position;
                }
                catch
                {

                }
            }
            return false;
        }
        public Status getCurrentStatus()
        {
            Status s = new Status();
            using (WebClient wc = new WebClient())
            {
                try
                {
                    String res = wc.DownloadString("http://localhost:" + portNumber + "/currentSong");
                    foreach (string l in res.Split(new string[] { "\n" }, StringSplitOptions.None))
                    {
                        string[] kv = l.Split(' ');
                        switch (kv[0])
                        {
                            case "status:":
                                s.State = kv[1];
                                break;
                            case "songName:":
                                s.SongName = l.Substring(10);
                                break;
                            case "artistName:":
                                s.ArtistName = l.Substring(12);
                                break;
                            case "albumName:":
                                s.AlbumName = l.Substring(11);
                                break;
                            case "trackNum:":
                                s.TrackNo = int.Parse(kv[1]);
                                break;
                            case "artURL:":
                                s.ArtURL = kv[1];
                                break;
                            case "calculatedDuration:":
                                s.Duration = double.Parse(kv[1]);
                                break;
                            case "position:":
                                s.Position = double.Parse(kv[1]);
                                break;
                            default:
                                break;
                        }
                    }
                    downloadArt(s);
                    return s;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
        public void downloadArt(Status status)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    if (!File.Exists("album/" + status.ArtistName + " - " + status.AlbumName + ".jpg"))
                    {
                        wc.DownloadFile(status.ArtURL, "album/" +status.ArtistName + " - " + status.AlbumName + ".jpg");
                    }
                }
                catch
                {
                }
            }
        }
        public void volUp()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadData("http://localhost:" + portNumber + "/volup");
                }
                catch
                {
                }
            }
        }
        public void volDown()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadData("http://localhost:" + portNumber + "/voldown");
                }
                catch
                {
                }
            }
        }
        public void volUpSmall()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    volume = int.Parse(wc.DownloadString("http://localhost:" + portNumber + "/volume").Split(' ')[1]);
                    wc.DownloadData("http://localhost:" + portNumber + "/setVolume?" + (volume + 2));
                }
                catch
                {
                }
            }
        }
        public void volDownSmall()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    volume = int.Parse(wc.DownloadString("http://localhost:" + portNumber + "/volume").Split(' ')[1]);
                    wc.DownloadData("http://localhost:" + portNumber + "/setVolume?" + (volume - 2));
                }
                catch
                {
                }
            }
        }
        public void back(ref Status status)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadData("http://localhost:" + portNumber + "/previous");
                    Form1.updateTime = 3000;
                    Form1.refreshTime = 0;
                }
                catch
                {
                }
            }
        }
        public void forward(ref Status status)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadData("http://localhost:" + portNumber + "/next");
                    Form1.updateTime = 3000;
                    Form1.refreshTime = 0;
                }
                catch
                {
                }
            }
        }
        public void submitPlay(Status status)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    //need to create dynamic 
                }
                catch
                {
                }
            }
        }
    }
}
