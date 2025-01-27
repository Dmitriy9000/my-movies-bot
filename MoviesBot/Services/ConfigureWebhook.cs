﻿using Microsoft.Extensions.Options;
using MoviesBot.Config;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MoviesBot.Services
{
    public class ConfigureWebhook : IHostedService
    {
        private readonly ILogger<ConfigureWebhook> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly BotConfig _botConfig;

        public ConfigureWebhook(
            ILogger<ConfigureWebhook> logger,
            IServiceProvider serviceProvider,
            IOptions<BotConfig> botOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botConfig = botOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webhookAddress = $"{_botConfig.HostAddress}{_botConfig.Route}";
            _logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);

            await botClient.SetWebhookAsync(
                url: webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                secretToken: _botConfig.SecretToken,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Setting webhook: task done");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook on app shutdown
            _logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
