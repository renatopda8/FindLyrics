using System.IO;
using System.Net;

namespace FindLyrics
{
    public static class Extensions
    {
        public static string GetHtml(this string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return string.Empty;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return string.Empty;
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}