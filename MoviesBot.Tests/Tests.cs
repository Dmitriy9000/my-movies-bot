using Microsoft.Extensions.Options;
using MoviesBot.Config;
using MoviesBot.Services;

namespace MoviesBot.Tests
{
    public class Tests
    {
        TransmissionService _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new TransmissionService(Options.Create<BotConfig>(new BotConfig
            {
                TransmissionUrl = "http://192.168.4.121:9091/transmission/rpc",
                TransmissionBaseDirectory = "/volume1/video/transmission-downloads",
                TransmissionUsername = "asdf",
                TransmissionPassword = "fdsa"
            }), null);
        }

        [Test]
        public async Task AddDownload()
        {
            var result = await _sut.AddDownload("magnet:?xt=urn:btih:D125D52602A3015F2FDD0108254E5092B741830F&dn=Harvey.Birdman.Attorney.At.Law.Complete.720P.WEB-DL&tr=http%3A%2F%2Fp4p.arenabg.com%3A1337%2Fannounce&tr=udp%3A%2F%2F47.ip-51-68-199.eu%3A6969%2Fannounce&tr=udp%3A%2F%2F9.rarbg.me%3A2780%2Fannounce&tr=udp%3A%2F%2F9.rarbg.to%3A2710%2Fannounce&tr=udp%3A%2F%2F9.rarbg.to%3A2730%2Fannounce&tr=udp%3A%2F%2F9.rarbg.to%3A2920%2Fannounce&tr=udp%3A%2F%2Fopen.stealth.si%3A80%2Fannounce&tr=udp%3A%2F%2Fopentracker.i2p.rocks%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.cyberia.is%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.dler.org%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.internetwarriors.net%3A1337%2Fannounce&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.openbittorrent.com%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=udp%3A%2F%2Ftracker.pirateparty.gr%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.tiny-vps.com%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.torrent.eu.org%3A451%2Fannounce");
        }

        [Test]
        public async Task DeleteDownload()
        {
            var completed = await _sut.GetCompletedDownloads();
            var incomplete = await _sut.GetIncompleteDownloads();

            _sut.DeleteDownload(5);
        }

        [Test]
        public async Task GetDownloadsInProgress()
        {
            var result = await _sut.GetCompletedDownloads();

            _sut.DeleteDownload(5);
        }

        [Test]
        public async Task SearchTorrent()
        {
            var search = new PirateBayTorrentSearch();
            var torrents = search.Search("Birdman");
        }

        [Test]
        public async Task ExtractMagnet()
        {
            var search = new PirateBayTorrentSearch();
            var torrents = search.GetMagnetByUrl("https://thepiratebay10.info/torrent/36183589/Harvey.Birdman.Attorney.At.Law.Complete.720P.WEB-DL");
            ;
        }
    }
}