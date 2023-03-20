using System.Diagnostics;

namespace MoviesBot.Models
{
    [DebuggerDisplay("{Name} | {Uploaded} | {Size} | {SeedersCount}")]
    public class TorrentSearchResultPreview
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Uploaded { get; set; }
        public required string Size { get; set; }
        public required string SeedersCount { get; set; }
        public required string Href { get; set; }
    }
}
