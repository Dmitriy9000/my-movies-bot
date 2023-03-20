using Microsoft.Extensions.Options;
using MoviesBot.Config;
using MoviesBot.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoviesBot.Services
{
    public class UpdateHandlers
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandlers> _logger;
        private readonly PirateBayTorrentSearch _pirateBay;
        private readonly TransmissionService _transmission;
        private readonly BotConfig _config;

        public UpdateHandlers(
            ITelegramBotClient botClient,
            ILogger<UpdateHandlers> logger,
            PirateBayTorrentSearch pirateBay,
            TransmissionService transmission,
            IOptions<BotConfig> config)
        {
            _botClient = botClient;
            _logger = logger;
            _pirateBay = pirateBay;
            _transmission = transmission;
            _config = config.Value;
        }

        public Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }

        private async Task BotOnMessageReceived(
            Message message,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive message type: {MessageType}", message.Type);

            if (message.Text is not { } messageText)
            {
                UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);
                return;
            }

            // Check permissions
            if (_config.Owners != null && _config.Owners.Contains(message.From.Id))
            {
                var responseMessagesTask = messageText.Split(' ')[0] switch
                {
                    "/show_downloads" => ShowDownloads(message, cancellationToken),
                    "/show_library" => ShowLibrary(message, cancellationToken),
                    "/delete" => StartDelete(message, cancellationToken),
                    "/delete_downloading" => StartDeleteDownloading(message, cancellationToken),
                    "/search" => StartSearchTorrent(message, cancellationToken),
                    _ => NumberOrUsage(message, cancellationToken)
                };

                var responses = await responseMessagesTask;

                foreach (var response in responses)
                {
                    await SendMessage(message.Chat.Id, response);
                }
            }
            else
            {
                await SendMessage(message.Chat.Id, "Access denied. Please contact bot's owner.");
            }
        }

        async Task<string[]> StartDelete(Message message, CancellationToken cancellationToken)
        {
            try
            {
                UserCache.UpdateState(message.Chat.Id, UserChatState.StartDeleteMovie);

                var downloads = await _transmission.GetCompletedDownloads();
                UserCache.UpdateLibrary(message.Chat.Id, downloads);
                var formatedMessage = MessageFormating.FormatCompleteDownloadsMessage(downloads);

                return new string[] { formatedMessage, "Please enter movie number (or few numbers delimeted by spaces) to delete (0 for cancel):" };
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> StartDeleteDownloading(Message message, CancellationToken cancellationToken)
        {
            try
            {
                UserCache.UpdateState(message.Chat.Id, UserChatState.StartDeleteDownloadingMovie);

                var downloads = await _transmission.GetIncompleteDownloads();
                UserCache.UpdateDownloading(message.Chat.Id, downloads);
                var formatedMessage = MessageFormating.FormatIncompleteDownloadsMessage(downloads);

                return new string[] { formatedMessage, "Please enter movie number (or few numbers delimeted by spaces) to delete (0 for cancel):" };
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> DeleteLibraryMovie(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var moviesToDelete = message.Text.ParseListOfInts();
                if (moviesToDelete.Any())
                {
                    if (!moviesToDelete.Contains(0))
                    {
                        var result = new string[moviesToDelete.Count()];

                        for (var i = 0; i < moviesToDelete.Count(); i++)
                        {
                            var selectedMovie = moviesToDelete[i];
                            var movie = UserCache.Library[message.Chat.Id][selectedMovie - 1];
                            _transmission.DeleteDownload(movie.Id);

                            result[i] = $"Library movie {movie.Name} has been deleted";
                        }

                        UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                        return result;
                    }
                    else
                    {
                        UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                        return new string[] { $"Delete cancelled" };
                    }
                }
                else
                {
                    UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                    return new string[] { "Doesn't look like a number or series of numbers. Try again." };
                }
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> DeleteDownloadingMovie(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var moviesToDelete = message.Text.ParseListOfInts();
                if (moviesToDelete.Any())
                {
                    if (!moviesToDelete.Contains(0))
                    {
                        var result = new string[moviesToDelete.Count()];

                        for (var i=0; i< moviesToDelete.Count(); i++)
                        {
                            var selectedMovie = moviesToDelete[i];
                            var movie = UserCache.Downloading[message.Chat.Id][selectedMovie - 1];

                            _transmission.DeleteDownload(movie.Id);

                            result[i] = $"Downloading movie {movie.Name} has been deleted";
                        }

                        UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                        return result;
                    }
                    else
                    {
                        UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                        return new string[] { $"Delete cancelled" };
                    }
                }
                else
                {
                    UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);
                    return new string[] { "Doesn't look like a number or series of numbers. Try again." };
                }
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> StartSearchTorrent(Message message, CancellationToken cancellationToken)
        {
            try
            {
                UserCache.UpdateState(message.Chat.Id, UserChatState.StartSearchMovie);

                return new string[] { "Please enter search prompt:" };
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> SearchTorrent(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var searchQuery = message.Text;

                var searchResult = _pirateBay.Search(searchQuery);

                if (searchResult.Any())
                {
                    UserCache.UpdateSearch(message.Chat.Id, searchResult);
                    UserCache.UpdateState(message.Chat.Id, UserChatState.SelectingMovieToDownload);

                    var formatedMessage = MessageFormating.FormatSearchResultMessage(searchResult);

                    return new string[] { formatedMessage };
                }
                else
                {
                    return new string[] { "No torrent found, try another prompt" };
                }
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> ShowDownloads(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var downloads = await _transmission.GetIncompleteDownloads();
                var formatedMessage = MessageFormating.FormatIncompleteDownloadsMessage(downloads);

                return new string[] { formatedMessage };
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> DownloadMovie(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var moviesToDownload = message.Text.ParseListOfInts();
                if (moviesToDownload.Any())
                {
                    if (!moviesToDownload.Contains(0))
                    {
                        var result = new string[moviesToDownload.Count()];

                        for (var i = 0; i < moviesToDownload.Count(); i++)
                        {
                            var selectedMovie = moviesToDownload[i];
                            var movie = UserCache.Search[message.Chat.Id][selectedMovie - 1];

                            var magnet = _pirateBay.GetMagnetByUrl(movie.Href);

                            await _transmission.AddDownload(magnet);

                            result[i] = $"{movie.Name} has been added to queue";
                        }

                        UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                        return result;
                    }
                    else
                    {
                        UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                        return new string[] { $"Search cancelled" };
                    }
                }
                else
                {
                    UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                    return new string[] { "Doesn't look like a number or series of numbers. Try again." };
                }
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        async Task<string[]> ShowLibrary(Message message, CancellationToken cancellationToken)
        {
            try
            {
                var downloads = await _transmission.GetCompletedDownloads();
                var formatedMessage = MessageFormating.FormatCompletedDownloadsMessage(downloads);

                return new string[] { formatedMessage };
            }
            catch (Exception e)
            {
                return new string[] { $"Error: {e.Message}" };
            }
        }

        Task<string[]> NumberOrUsage(Message message, CancellationToken cancellationToken)
        {
            try
            {
                if (UserCache.State.TryGetValue(message.Chat.Id, out var state) && state != UserChatState.AwaitingCommand)
                {
                    if (state == UserChatState.StartSearchMovie)
                    {
                        return SearchTorrent(message, cancellationToken);
                    }
                    else if (state == UserChatState.StartDeleteMovie)
                    {
                        return DeleteLibraryMovie(message, cancellationToken);
                    }
                    else if (state == UserChatState.StartDeleteDownloadingMovie)
                    {
                        return DeleteDownloadingMovie(message, cancellationToken);
                    }
                    else if (state == UserChatState.SelectingMovieToDownload)
                    {
                        return DownloadMovie(message, cancellationToken);
                    }
                    else
                    {
                        // Other multisteps workflows
                        UserCache.UpdateState(message.Chat.Id, UserChatState.AwaitingCommand);

                        return Task.FromResult(new string[]
                        {
                            "Error - something really wrong"
                        });
                    }
                }
                else
                {
                    const string outcomeMessage = "Usage:\n" +
                                     "/show_library       - show my library\n" +
                                     "/search           - enter prompt to search for torrent\n" +
                                     "/delete - delete movie\n" +
                                     "/delete_downloading - delete movie from download queue\n" +
                                     "/show_downloads  - show current downloads\n";
                    return Task.FromResult(new string[]
                       {
                            outcomeMessage
                       });
                }
            }
            catch (Exception e)
            {
                return Task.FromResult(new string[] { $"Error: {e.Message}" });
            }
        }

        private async Task SendMessage(long chatId, string textMessage)
        {
            var messages = textMessage.SplitOnLength(4096);

            foreach (var m in messages)
            {
                await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: m,
                        replyMarkup: new ReplyKeyboardRemove());
            }
        }

        private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unknown update type: {UpdateType}", update);

            return Task.CompletedTask;
        }
    }
}
