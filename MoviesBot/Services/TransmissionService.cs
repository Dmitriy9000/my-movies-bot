using Microsoft.Extensions.Options;
using MoviesBot.Config;
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;

namespace MoviesBot.Services
{
    public class TransmissionService
    {
        private readonly BotConfig _botConfig;
        private readonly ILogger<TransmissionService> _logger;

        public TransmissionService(
            IOptions<BotConfig> config,
            ILogger<TransmissionService> logger)
        {
            _botConfig = config.Value ?? throw new ArgumentNullException(nameof(config));
            _logger = logger;
        }

        private Client GetClient()
        {
            return new Client(_botConfig.TransmissionUrl, login: _botConfig.TransmissionUsername, password: _botConfig.TransmissionPassword);
        }

        public async Task<Models.TorrentInfo> AddDownload(string magnet)
        {
            try
            {
                var client = GetClient();

                // API requests absolute path, not sure how we can provide it as downloads are stored on attached docker volume, so no path selection so far

                var addResponse = await client.TorrentAddAsync(new NewTorrent
                {
                    Filename = magnet,
                });

                return new Models.TorrentInfo
                {
                    Id = addResponse.ID,
                    Name = addResponse.Name,
                    Progress = 0
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to add torrent: {e.Message}", e);

                throw;
            }
        }

        public async Task<List<Models.TorrentInfo>> GetCompletedDownloads()
        {
            try
            {
                var client = GetClient();

                var allTorrents = await client.TorrentGetAsync(TorrentFields.ALL_FIELDS);

                var progressing = allTorrents.Torrents
                    .Where(c => c.PercentDone == 1)
                    .Select(c => new Models.TorrentInfo()
                    {
                        Id = c.ID,
                        Name = c.Name,
                        Progress = c.PercentDone,
                        DoneDate = c.DoneDate,
                        PeersConnected = c.PeersConnected,
                        TotalSize = c.TotalSize
                    })
                    .ToList();

                return progressing;
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to get completed downloads torrent: {e.Message}", e);

                throw;
            }
        }

        public async Task<List<Models.TorrentInfo>> GetIncompleteDownloads()
        {
            try
            {
                var client = GetClient();

                var allTorrents = await client.TorrentGetAsync(TorrentFields.ALL_FIELDS);

                var progressing = allTorrents.Torrents
                    .Where(c => c.PercentDone != 1)
                    .Select(c => new Models.TorrentInfo()
                    {
                        Id = c.ID,
                        Name = c.Name,
                        Progress = c.PercentDone,
                        DoneDate = c.DoneDate,
                        PeersConnected = c.PeersConnected,
                        TotalSize = c.TotalSize
                    })
                    .ToList();

                return progressing;
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to get incomplete downlaods: {e.Message}", e);

                throw;
            }
        }

        public void DeleteDownload(int id)
        {
            try
            {
                var client = GetClient();
                client.TorrentRemove(new[] { id }, deleteData: true);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to delete torrent: {e.Message}", e);

                throw;
            }
        }
    }
}
