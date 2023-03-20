namespace MoviesBot.Models
{
    public enum UserChatState
    {
        AwaitingCommand,
        StartSearchMovie,
        SelectingMovieToDownload,
        StartDeleteMovie,
        DeleteMovie,
        StartDeleteDownloadingMovie,
    }
}
