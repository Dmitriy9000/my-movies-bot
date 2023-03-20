namespace MoviesBot.Config
{
    public class BotConfig
    {
        public string BotToken { get; init; } = default!;
        public string HostAddress { get; init; } = default!;
        public string Route { get; init; } = default!;
        public string SecretToken { get; init; } = default!;

        public string TransmissionUrl { get; set; } = default!;
        public string TransmissionUsername { get; set; } = default!;
        public string TransmissionPassword { get; set; } = default!;

        public string TransmissionBaseDirectory { get; set; } = default!;
        public long[] Owners { get; set; }
    }
}
