using MoviesBot.Models;

namespace MoviesBot.Services
{
    public static class UserCache
    {
        public static Dictionary<long, UserChatState> State { get; } = new Dictionary<long, UserChatState>();

        public static Dictionary<long, TorrentSearchResultPreview[]> Search { get; } = new Dictionary<long, TorrentSearchResultPreview[]>();

        public static Dictionary<long, List<TorrentInfo>> Downloading { get; } = new Dictionary<long, List<TorrentInfo>>();

        public static Dictionary<long, List<TorrentInfo>> Library { get; } = new Dictionary<long, List<TorrentInfo>>();

        public static void UpdateState(long chatId, UserChatState state)
        {
            if (State.ContainsKey(chatId))
            {
                State[chatId] = state;
            }
            else
            {
                State.Add(chatId, state);
            }
        }

        public static void UpdateSearch(long chatId, TorrentSearchResultPreview[] input)
        {
            if (Search.ContainsKey(chatId))
            {
                Search[chatId] = input;
            }
            else
            {
                Search.Add(chatId, input);
            }
        }

        public static void UpdateLibrary(long chatId, List<TorrentInfo> input)
        {
            if (Library.ContainsKey(chatId))
            {
                Library[chatId] = input;
            }
            else
            {
                Library.Add(chatId, input);
            }
        }

        public static void UpdateDownloading(long chatId, List<TorrentInfo> input)
        {
            if (Downloading.ContainsKey(chatId))
            {
                Downloading[chatId] = input;
            }
            else
            {
                Downloading.Add(chatId, input);
            }
        }
    }
}
