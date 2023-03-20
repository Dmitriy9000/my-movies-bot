using MoviesBot.Models;
using System.Text;

namespace MoviesBot.Services
{
    public static class MessageFormating
    {
        public static string FormatIncompleteDownloadsMessage(List<TorrentInfo> input)
        {
            if (input.Count == 0)
            {
                return "There are no active downloads";
            }

            var sb = new StringBuilder();
            var i = 1;

            foreach (var torrent in input)
            {
                sb.AppendLine($"{i}. {torrent.Name}, Progress: {torrent.Progress:P}, Size: {Extensions.BytesToString(torrent.TotalSize)}, Seeds: {torrent.PeersConnected}");

                i++;
            }

            return sb.ToString();
        }

        public static string FormatCompleteDownloadsMessage(List<TorrentInfo> input)
        {
            if (input.Count == 0)
            {
                return "There are no movies in library";
            }

            var sb = new StringBuilder();
            var i = 1;

            foreach (var torrent in input)
            {
                sb.AppendLine($"{i}. {torrent.Name}, Size: {Extensions.BytesToString(torrent.TotalSize)}, Seeds: {torrent.PeersConnected}");

                i++;
            }

            return sb.ToString();
        }

        public static string FormatCompletedDownloadsMessage(List<TorrentInfo> input)
        {
            if (input.Count == 0)
            {
                return "There are no entries in library so far";
            }

            var sb = new StringBuilder();
            var i = 1;

            foreach (var torrent in input)
            {
                var completed = DateTimeOffset.FromUnixTimeSeconds(torrent.DoneDate);
                var time = completed.DateTime.ToShortTimeString();
                var date = completed.DateTime.ToShortDateString();

                sb.AppendLine($"{i}. {torrent.Name}, Size: {Extensions.BytesToString(torrent.TotalSize)}, Completed: {date} {time}");

                i++;
            }

            return sb.ToString();
        }

        internal static string FormatSearchResultMessage(TorrentSearchResultPreview[] searchResult)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type number of movie to add to download list. To cancel search type 0:");

            foreach (var torrent in searchResult)
            {
                sb.AppendLine($"{torrent.Id}. {torrent.Name}, Size: {torrent.Size}, Created: {torrent.Uploaded}, Seeders: {torrent.SeedersCount}");
            }

            return sb.ToString();
        }
    }
}
