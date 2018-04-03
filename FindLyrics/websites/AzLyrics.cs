using System.Text.RegularExpressions;

namespace FindLyrics.websites
{
    public class AzLyrics : ILyrics
    {
        public string SearchLyricsUrl(string artistName, string trackName)
        {
            string playingInfo = $"{artistName} {trackName}".Replace(' ', '+');
            string searchUrl = $"https://search.azlyrics.com/search.php?q={playingInfo}";
            string html = searchUrl.GetHtml();
            return GetUrlFromHtml(html);
        }

        private string GetUrlFromHtml(string html)
        {
            Regex regex = new Regex("1\\. <a href\\s*=\\s*(?:\"(?<url>[^\"]*)\"|(?<url>\\S+))", RegexOptions.IgnoreCase);
            Match match = regex.Match(html);
            return match.Groups["url"]?.Value ?? string.Empty;
        }
    }
}