using Microsoft.Extensions.Options;

namespace MoviesBot
{
    public static class Extensions
    {
        public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            var o = serviceProvider.GetService<IOptions<T>>();
            if (o is null)
                throw new ArgumentNullException(nameof(T));

            return o.Value;
        }

        public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(
            this IEndpointRouteBuilder endpoints,
            string route)
        {
            var controllerName = typeof(T).Name.Replace("Controller", "", StringComparison.Ordinal);
            var actionName = typeof(T).GetMethods()[0].Name;

            return endpoints.MapControllerRoute(
                name: "bot_webhook",
                pattern: route,
                defaults: new { controller = controllerName, action = actionName });
        }

        public static IEnumerable<string> SplitOnLength(this string input, int length)
        {
            int index = 0;
            while (index < input.Length)
            {
                if (index + length < input.Length)
                    yield return input.Substring(index, length);
                else
                    yield return input.Substring(index);

                index += length;
            }
        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static List<int> ParseListOfInts(this string input)
        {
            try
            {
                var splited = input.Split(' ');
                return splited.Select(c => Convert.ToInt32(c)).ToList();
            }
            catch
            {
                return new List<int>();
            }
        }
    }
}
