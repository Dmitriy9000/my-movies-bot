using HtmlAgilityPack;
using MoviesBot.Models;

namespace MoviesBot.Services
{
    public class PirateBayTorrentSearch
    {
        const string domain = "https://thepiratebay10.info";
        const string videoCategory = "200";

        public TorrentSearchResultPreview[] Search(string prompt)
        {
            try
            {
                var link = $"{domain}/search/{prompt}/1/99/{videoCategory}";
                var web = new HtmlWeb();
                var doc = new HtmlDocument();

                doc = web.Load(link);
                var result = new List<TorrentSearchResultPreview>();

                var tableRows = doc.DocumentNode.SelectNodes("//table//tr");
                var i = 1;

                foreach (var row in tableRows)
                {
                    if (row.InnerHtml.Contains("<th>"))
                        continue;

                    var cols = row.Descendants("td").ToList();
                    if (cols.Count != 8)
                        continue;

                    var href = cols[1].SelectNodes("a").First().Attributes["href"].Value;

                    result.Add(new TorrentSearchResultPreview
                    {
                        Id = i,
                        Href = href,
                        Name = cols[1].InnerText.Trim(),
                        Uploaded = cols[2].InnerText.Replace("&nbsp;", " "),
                        Size = cols[4].InnerText.Replace("&nbsp;", " "),
                        SeedersCount = cols[5].InnerText
                    });

                    i++;
                }

                return result.Take(20).ToArray();
            }
            catch ()
            {
                throw;
            }
        }

        public string GetMagnetByUrl(string url)
        {
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var link = doc.DocumentNode
                  .Descendants("a")
                  .First(x => x.Attributes["title"] != null
                           && x.Attributes["title"].Value == "Get this torrent");

                var hrefValue = link.Attributes["href"].Value;

                return hrefValue;

            }
            catch ()
            {
                throw;
            }
        }
    }
}
