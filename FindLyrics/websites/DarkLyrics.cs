using System;
using System.Text.RegularExpressions;

namespace FindLyrics.websites
{
    public class DarkLyrics : ILyrics
    {
        public string SearchLyricsUrl(string artistName, string trackName)
        {
            string playingInfo = $"{artistName} {trackName}".Replace(' ', '+');
            string searchUrl = $"http://www.darklyrics.com/search?q={playingInfo}";
            string html = searchUrl.GetHtml();
            return GetUrlFromHtml(html);
        }

        private string GetUrlFromHtml(string html)
        {
            Regex regex = new Regex("<h2><a href=\"(?<url>.*)\" target=\"_blank\" >", RegexOptions.IgnoreCase);
            Match match = regex.Match(html);
            string url = match.Groups["url"]?.Value;
            return string.IsNullOrWhiteSpace(url) ? string.Empty : $"http://www.darklyrics.com/{url}";
        }
    }
}