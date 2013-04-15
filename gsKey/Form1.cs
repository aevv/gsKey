using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using gsKey.APIProxy;
using System.Diagnostics;
using System.IO;

namespace gsKey
{
    public partial class Form1 : Form
    {

        //win32
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        Status current;
        bool min = false;
        int iws = 0;
        string nowPlaying = "Now playing ";
        Size screen;
        API api = new API();
        bool imageInvalid = true;
        Image album;


        public Form1()
        {
            InitializeComponent();
            registerKeys();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            screen = Screen.FromControl(this).Bounds.Size;
            this.FormBorderStyle = FormBorderStyle.None;
            iws = GetWindowLong(this.Handle, -20);
            int n = iws | 0x20 | 0x80000;
            SetWindowLong(this.Handle, -20, n);
            this.TopMost = true;
            this.Opacity = 0.8;
            this.Location = new Point(screen.Width - 200, screen.Height - 114);
            this.Size = new Size(200, 75);
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            current = api.getCurrentStatus();
            Thread t = new Thread(new ThreadStart(poll));
            t.Start();
            if (!Directory.Exists("album"))
            {
                Directory.CreateDirectory("album");
            }
        }
        private void handleKey(Keys vk)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    if (vk == Keys.Z)
                        api.togglePlay(ref current);
                    else if (vk == Keys.Up)
                        api.volUp();
                    else if (vk == Keys.Down)
                        api.volDown();
                    else if (vk == Keys.O)
                    {
                        api.volDownSmall();
                    }
                    else if (vk == Keys.P)
                    {
                        api.volUpSmall();
                    }
                    else if (vk == Keys.Left)
                    {
                        api.back(ref current);
                    }
                    else if (vk == Keys.Right)
                    {
                        api.forward(ref current);
                    }
                    else if (vk == Keys.M)
                    {
                        if (min == true)
                        {
                            max();
                        }
                        else
                        {
                            this.WindowState = FormWindowState.Minimized;
                            Form1_Resize(null, null);
                        }
                    }
                    else if (vk == Keys.N)
                    {
                        notifyIcon1.Visible = false;
                        Environment.Exit(0);
                    }
                    wc.CachePolicy = new System.Net.Cache.RequestCachePolicy();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                GC.Collect();
            }
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                Keys vk = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                handleKey(vk);
            }
            base.WndProc(ref m);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "gsKey";
                notifyIcon1.Text = "gsKey";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                System.Drawing.Icon icon;
                System.IO.Stream st;
                System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                st = a.GetManifestResourceStream("gsKey.aevvhead.ico");
                icon = new System.Drawing.Icon(st);
                notifyIcon1.Icon = icon;
                notifyIcon1.DoubleClick += new EventHandler(delegate(object s, EventArgs ea)
                {
                    max();
                });
                this.Hide();
                min = true;
                this.ShowInTaskbar = true;
                iws = GetWindowLong(this.Handle, -20);
                int n = iws | 0x20 | 0x80000;
                SetWindowLong(this.Handle, -20, n);
            }
            registerKeys();
        }
        private void mi_Click(object sender, EventArgs ea)
        {
            notifyIcon1.Visible = false;
            Environment.Exit(0);
        }
        private void ma_Click(object sender, EventArgs ea)
        {
            max();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
        }
        private void max()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = false;
            min = false;
            notifyIcon1.Visible = false;
            this.Location = new Point(screen.Width - 200, screen.Height - 114);
            this.Size = new Size(200, 75);
            iws = GetWindowLong(this.Handle, -20);
            int n = iws | 0x20 | 0x80000;
            SetWindowLong(this.Handle, -20, n);
            registerKeys();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                draw(e.Graphics);
            }
            catch
            {
            }
        }
        private float scrollLoc = 0;
        private float scrollLoc2 = 500;
        bool spawn = true, spawn2 = true;
        Font font = new Font(new FontFamily("Segoe UI"), 10);
        private void draw(Graphics g)
        {
            g.Clear(Color.DarkGray);
            SizeF size = g.MeasureString(nowPlaying, font);
            double per = current.Position / current.Duration;
            float bound = size.Width - this.Width;
            if (scrollLoc <= -bound)
            {
                spawn = true;
                if (spawn2)
                {
                    scrollLoc2 = this.Width + 20;
                    spawn2 = false;
                }
            }
            if (scrollLoc2 <= -bound)
            {
                spawn2 = true;
                if (spawn)
                {
                    scrollLoc = this.Width + 20;
                    spawn = false;
                }
            }
            g.DrawString(nowPlaying, font, new SolidBrush(Color.Black), new PointF(scrollLoc + 1, 1));
            g.DrawString(nowPlaying, font, new SolidBrush(Color.White), new PointF(scrollLoc, 0));
            g.DrawString(nowPlaying, font, new SolidBrush(Color.Black), new PointF(scrollLoc2 + 1, 1));
            g.DrawString(nowPlaying, font, new SolidBrush(Color.White), new PointF(scrollLoc2, 0));
            if (album != null)
            {
                g.DrawImage(album, new Rectangle(3, 17, 55, 55));
            }
            TimeSpan t = TimeSpan.FromMilliseconds(current.Position);
            string cur = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            TimeSpan t2 = TimeSpan.FromMilliseconds(current.Duration);
            string dur = string.Format("{0:D2}:{1:D2}", t2.Minutes, t2.Seconds);
            g.DrawString(cur + " / " + dur, font, new SolidBrush(Color.Black), new PointF(61, 26));
            g.DrawString(cur + " / " + dur, font, new SolidBrush(Color.White), new PointF(60, 25));

            g.DrawLine(new Pen(new SolidBrush(Color.Black), 4), new Point(60, 50), new Point(190, 50));
            int length = (int)(130.0 * per);
            g.DrawLine(new Pen(new SolidBrush(Color.White), 2), new Point(60, 50), new Point(60 + length, 50));
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 3, this.Height));
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, this.Width, 3));
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, this.Height - 3, this.Width, 3));
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(this.Width - 3, 0, 3, this.Height));
        }
        double oldPos;
        public static double refreshTime = 0;
        public static double updateTime = 30000;
        private void poll()
        {
            Stopwatch stop = new Stopwatch();
            long change = 0;
            while (true)
            {
                try
                {
                    oldPos = current.Position;
                    if (imageInvalid)
                    {
                        try
                        {
                            album = Image.FromFile("album/" + current.ArtistName + " - " + current.AlbumName + ".jpg");
                        }
                        catch
                        {
                            album = Image.FromFile("default.jpg");
                        }
                    }
                    if (current.Position < (oldPos - 2000))
                        current = api.getCurrentStatus();
                    if (current.Playing)
                    {
                        current.Position += change;
                        current.Life += change;
                        if (current.Life > (current.Duration / 10))
                        {
                            current.Life = long.MinValue;
                            api.submitPlay(current);
                        }
                        if (current.Position >= current.Duration)
                        {
                            nowPlaying = "Finished " + current.SongName + " by " + current.ArtistName + " on " + current.AlbumName;
                            imageInvalid = true;
                            current.Playing = false;
                            current = api.getCurrentStatus();
                            Thread.Sleep(200);
                            continue;
                        }
                        nowPlaying = "Now playing " + current.SongName + " by " + current.ArtistName + " on " + current.AlbumName;
                    }
                    else
                    {
                        nowPlaying = "Paused " + current.SongName + " by " + current.ArtistName + " on " + current.AlbumName;
                    }
                    canvas.Invalidate();

                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate() { this.TopMost = true; });
                    }
                    stop.Start();
                    Thread.Sleep(100);
                    stop.Stop();
                    change = stop.ElapsedMilliseconds;
                    stop.Reset();
                    refreshTime += change;
                    if (refreshTime > updateTime)
                    {
                        refreshTime = 0;
                        long life = current.Life;
                        string title = current.SongName;
                        current = api.getCurrentStatus();
                        if (title.Equals(current.SongName))
                            current.Life = life;
                        if (updateTime == 3000)
                            imageInvalid = true;
                        updateTime = 30000;
                    }
                    scrollLoc -= 5.0f;
                    scrollLoc2 -= 5.0f;
                }
                catch
                {
                }
            }
        }
        private void registerKeys()
        {
            GlobalHotkey k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.Z, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.Up, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.Down, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.O, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.P, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.M, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.N, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.Left, this);
            k.Register();
            k = new GlobalHotkey(Constants.CTRL | Constants.ALT | Constants.SHIFT, Keys.Right, this);
            k.Register();
        }
    }
}
