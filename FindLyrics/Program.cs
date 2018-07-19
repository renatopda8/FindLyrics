using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FindLyrics.websites;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace FindLyrics
{
    public class Program
    {
        static Program()
        {
            SetDebug();
        }

        private static SpotifyWebAPI _spotify;
        private static List<ILyrics> _lyricsWebsites;

        public static void Main(string[] args)
        {
            string artistName = string.Empty;
            string trackName = string.Empty;
            try
            {
                PlaybackContext pc = Spotify.GetPlayback();
                if (pc == null)
                {
                    throw new Exception("PlaybackContext null");
                }

                if (pc.Error != null)
                {
                    throw new Exception($"{pc.Error.Status} - {pc.Error.Message}");
                }

                if (pc.Item == null)
                {
                    throw new Exception("FullTrack null");
                }

                artistName = pc.Item.Artists?.FirstOrDefault()?.Name;
                trackName = pc.Item.Name;
                foreach (ILyrics lyrics in LyricsWebsites)
                {
                    string lyricsResult = lyrics.SearchLyricsUrl(artistName, trackName);
                    if (string.IsNullOrWhiteSpace(lyricsResult))
                    {
                        continue;
                    }

                    Process.Start(lyricsResult);
                    return;
                }

                MessageBox.Show($"Artist: {artistName}\nTrack: {trackName}", "Lyrics not found");
            }
            catch (Exception e)
            {
                if (EhDebug)
                {
                    throw;
                }

                string message = e.ToString();
                if (!string.IsNullOrWhiteSpace(artistName) || !string.IsNullOrWhiteSpace(trackName))
                {
                    message = $"Artist: {artistName}\nTrack: {trackName}\n{message}";
                }

                Trace.TraceError(message);
                MessageBox.Show(message, "Exception");
            }
        }

        /// <summary>
        /// Método condicional para definir que a execução é realizada em debug
        /// </summary>
        [Conditional("DEBUG")]
        private static void SetDebug()
        {
            EhDebug = true;
        }

        /// <summary>
        /// Define se a execução está sendo realizada em debug
        /// </summary>
        public static bool EhDebug { get; set; }

        /// <summary>
        /// Sites de letras de músicas
        /// </summary>
        private static List<ILyrics> LyricsWebsites =>
            _lyricsWebsites ?? (_lyricsWebsites = new List<ILyrics> {new AzLyrics(), new DarkLyrics()});

        /// <summary>
        /// Web API do Spotify
        /// </summary>
        private static SpotifyWebAPI Spotify
        {
            get
            {
                if (_spotify == null)
                {
                    WebAPIFactory webApiFactory = new WebAPIFactory("http://localhost", 8000, "c7af116523b3453ea822e36cc694d7f5",
                    Scope.UserReadPlaybackState, TimeSpan.FromSeconds(20));
                    _spotify = webApiFactory.GetWebApi()?.Result;
                }
                
                return _spotify;
            }
        }
    }
}