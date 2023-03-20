# Movies bot

## Use case

It happens that you came across description of some movie and want to watch it later, but it's a pretty long sequence of action - open browser, find torrent you want, open magnet link in torrent client. Not to say you need access to your computer and all you have now is a smartphone! With the Movies bot you can do it right away: open Telegram, select bot chat, search movie by its name, add to queue. It's done. Once you at home, very likely the movie is already downloaded, so you can start watching it. Another benefit - you can manage your media library through your smartphone.

![menu](https://kudinov.dev/images/movies-bot/menu.png)

## Repository

This repository contains code of Movies bot for Telegram. The bot allows you to search torrents by prompt and add selected torrents to the downloading queue of your Transmission torrent client. You can also remove downloading torrents and already downloaded torrents as well as their content.

You may setup Transmission to download torrents into particular directory where your video server will find it (for example somewhere on your NAS), or just to your local machine.

![search](https://kudinov.dev/images/movies-bot/search-torrent.png)

Search is performed on Pirate Bay in the video category, bot will show you top 20 torrents available for your request sorted by number of active peers. From there you can select number of the torrent you want to add to downloading queue. While torrent is downloading you can query for its progress.

## Configuration

### Prerequisites

- Telegram bot (you can create one using BotFather) - you will need bot token, secret token
- Public address for bot callbacks - if you don't own a domain you can create a public address using tool like ngrok
- Your Telegram user id - only users from the configuration will be able to communicate with your bot. You can add few users as administrators of your bot.
- Transmission instance - it could be running on your local machine, docker, NAS or anywhere else. All you need is username, password and URL bot can reach

To configure bot, update corresponding values in appsettings.*.json file. Then run the bot. Find it in Telegram directory and send him a message. Configuration can be tricky and depends on your environment. I will be updating the readme as new questions arise. Feel free to push updates as well.

![configuration](https://kudinov.dev/images/movies-bot/config.png)

## Future aspirations

- Manage administrators through the bot interface
- Add new torrent sites
- Pass the task cancellation tokens through the endpoints

## Feedback and more information

The code is open source and licensed under MIT License. Feel free to send me any feedback you may have through Github. PRs are welcome!
