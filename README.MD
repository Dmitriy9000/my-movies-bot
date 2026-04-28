# Movies Bot

A Telegram bot that lets you search [Pirate Bay](https://thepiratebay.org) for torrents and manage downloads through your [Transmission](https://transmissionbt.com) client — all from your phone.

![menu](https://kudinov.dev/images/movies-bot/menu.png)

## Use Case

When you come across a movie you want to watch, the usual flow is: open a browser, find a torrent, open the magnet link in a torrent client. With Movies Bot you do it from Telegram in seconds. By the time you get home, the movie is likely already downloaded.

You may set up Transmission to download torrents into a directory where your media server picks them up automatically (e.g. a NAS), or just to your local machine.

## Features

- Search Pirate Bay by movie name (top 20 results, sorted by active peers)
- Add torrents to Transmission download queue
- View active downloads with progress
- View your completed library
- Delete movies from the library or cancel active downloads

![search](https://kudinov.dev/images/movies-bot/search-torrent.png)

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or Docker
- A running [Transmission](https://transmissionbt.com) instance (local, Docker, or NAS) with RPC enabled
- A Telegram bot token from [@BotFather](https://t.me/botfather)
- A public HTTPS URL for Telegram webhooks (e.g. via [ngrok](https://ngrok.com) or a domain)
- Your Telegram user ID (only listed users can use the bot — get it from [@userinfobot](https://t.me/userinfobot))

## Setup

### 1. Clone and configure

```bash
git clone <repo-url>
cd my-movies-bot
```

Copy the example settings file and fill in your values:

```bash
cp MoviesBot/appsettings.Development.json MoviesBot/appsettings.Development.json.local
# edit appsettings.Development.json with your credentials
```

![configuration](https://kudinov.dev/images/movies-bot/config.png)

### 2. Configuration reference

Edit `MoviesBot/appsettings.Development.json` (for local) or `appsettings.Production.json` (for Docker/production):

| Field | Description |
|---|---|
| `BotToken` | Telegram bot token from @BotFather |
| `HostAddress` | Public HTTPS URL where Telegram can reach the bot (e.g. `https://abc123.ngrok.io`) |
| `Route` | Webhook path, default `/bot` |
| `SecretToken` | Any random secret string — Telegram sends it on each webhook request for validation |
| `Owners` | Array of Telegram user IDs allowed to use the bot |
| `TransmissionUrl` | Transmission RPC endpoint, e.g. `http://192.168.1.100:9091/transmission/rpc` |
| `TransmissionUsername` | Transmission RPC username |
| `TransmissionPassword` | Transmission RPC password |
| `TransmissionBaseDirectory` | Path where Transmission stores downloaded files |

### 3. Run

**With .NET:**
```bash
cd MoviesBot
dotnet run
```

**With Docker:**
```bash
docker build -t movies-bot .
docker run -p 5000:5000 movies-bot
```

The bot registers its Telegram webhook automatically on startup and removes it on shutdown.

### 4. Use it

Find your bot in Telegram and send any message. Available commands:

| Command | Description |
|---|---|
| `/search` | Search for a torrent by name |
| `/show_downloads` | Show active downloads with progress |
| `/show_library` | Show completed downloads |
| `/delete` | Delete a movie from the library (removes files) |
| `/delete_downloading` | Cancel an active download (removes files) |

## Notes

- Only users whose Telegram IDs appear in `Owners` can interact with the bot.
- For Docker deployments, Transmission must be reachable from the container — use the host machine's IP instead of `localhost`.
- Transmission RPC must be enabled in its preferences (Remote access).

## Future Aspirations

- Manage administrators through the bot interface
- Add support for more torrent sites
- Pass task cancellation tokens through endpoints

## Contributing

PRs are welcome! Feel free to open an issue or submit a pull request on GitHub.

## License

MIT License — see [LICENSE](LICENSE) for details.
