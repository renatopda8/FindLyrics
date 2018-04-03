using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using FindLyrics.websites;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;

namespace FindLyrics
{
    public class Program
    {
        static Program()
        {
            SetDebug();
        }

        private static SpotifyLocalAPI _spotify;
        private static List<ILyrics> _lyricsWebsites;

        public static void Main(string[] args)
        {
            string artistName = string.Empty;
            string trackName = string.Empty;
            try
            {
                StatusResponse status = Spotify.GetStatus();
                artistName = status.Track.ArtistResource.Name;
                trackName = status.Track.TrackResource.Name;
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
        /// API Local do Spotify
        /// </summary>
        private static SpotifyLocalAPI Spotify
        {
            get
            {
                if (_spotify != null)
                {
                    return _spotify;
                }

                if (!SpotifyLocalAPI.IsSpotifyRunning())
                {
                    throw new Exception("Spotify not running");
                }

                if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
                {
                    throw new Exception("Spotify WebHelper not running");
                }

                _spotify = new SpotifyLocalAPI();
                if (!_spotify.Connect())
                {
                    throw new Exception("Failed to Connect with Spotify");
                }

                return _spotify;
            }
        }
    }
}