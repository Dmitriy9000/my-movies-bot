﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesBot.Config;

namespace MoviesBot.Filters
{
    /// <summary>
    /// Check for "X-Telegram-Bot-Api-Secret-Token"
    /// Read more: <see href="https://core.telegram.org/bots/api#setwebhook"/> "secret_token"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ValidateTelegramBotAttribute : TypeFilterAttribute
    {
        public ValidateTelegramBotAttribute()
            : base(typeof(ValidateTelegramBotFilter))
        {
        }

        private class ValidateTelegramBotFilter : IActionFilter
        {
            private readonly string _secretToken;

            public ValidateTelegramBotFilter(IOptions<BotConfig> options)
            {
                var botConfiguration = options.Value;
                _secretToken = botConfiguration.SecretToken;
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                Console.WriteLine("Valid request, X-Telegram-Bot-Api-Secret-Token is good");
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (!IsValidRequest(context.HttpContext.Request))
                {
                    Console.WriteLine("Invalid request, X-Telegram-Bot-Api-Secret-Token is invalid");

                    context.Result = new ObjectResult("\"X-Telegram-Bot-Api-Secret-Token\" is invalid")
                    {
                        StatusCode = 403
                    };
                }
            }

            private bool IsValidRequest(HttpRequest request)
            {
                var isSecretTokenProvided = request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var secretTokenHeader);
                if (!isSecretTokenProvided) return false;

                return string.Equals(secretTokenHeader, _secretToken, StringComparison.Ordinal);
            }
        }
    }
}
