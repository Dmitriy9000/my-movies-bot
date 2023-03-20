using Microsoft.AspNetCore.Mvc;
using MoviesBot.Filters;
using MoviesBot.Services;
using Telegram.Bot.Types;

namespace MoviesBot.Controllers
{
    public class BotController : ControllerBase
    {
        [HttpPost]
        [ValidateTelegramBot]
        public async Task<IActionResult> Post(
            [FromBody] Update update,
            [FromServices] UpdateHandlers handleUpdateService,
            CancellationToken cancellationToken)
        {
            await handleUpdateService.HandleUpdateAsync(update, cancellationToken);

            return Ok();
        }
    }
}
