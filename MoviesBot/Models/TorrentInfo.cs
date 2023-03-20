namespace MoviesBot.Models
{
    public class TorrentInfo
    {
        public required string Name { get; set; }
        public int Id { get; set; }
        public double Progress { get; set; }
        public long TotalSize { get; set; }
        public int PeersConnected { get; set; }
        public long DoneDate { get; set; }
    }
}
