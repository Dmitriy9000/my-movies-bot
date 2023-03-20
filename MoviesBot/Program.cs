using MoviesBot.Config;
using MoviesBot.Controllers;
using MoviesBot.Services;
using MoviesBot;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Environment: " + builder.Environment.EnvironmentName);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json").Build();

var botConfigurationSection = builder.Configuration.GetSection("BotConfiguration");

builder.Services.Configure<BotConfig>(botConfigurationSection);

var botConfiguration = botConfigurationSection.Get<BotConfig>();

builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfig? botConfig = sp.GetConfiguration<BotConfig>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

builder.Services.AddScoped<UpdateHandlers>();
builder.Services.AddScoped<TransmissionService>();
builder.Services.AddScoped<PirateBayTorrentSearch>();

builder.Services.AddHostedService<ConfigureWebhook>();

// Add services to the container.

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseAuthorization();

app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);
app.UseHttpLogging();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{


}

app.Run();