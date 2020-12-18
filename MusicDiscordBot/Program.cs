using DSharpPlus;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSharpPlus.CommandsNext;
using System.Reflection;
using MusicDiscordBot.Commands;
using Microsoft.Extensions.Configuration;

namespace MusicDiscordBot
{
    class Program
    {

        public static IConfigurationRoot Configuration { get; set; }

        public static DiscordClient Discord;

        static void Main(string[] args)
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
                                devEnvironmentVariable.ToLower() == "development";
            //Determines the working environment as IHostingEnvironment is unavailable in a console app

            var builder = new ConfigurationBuilder();
            // tell the builder to look for the appsettings.json file
            builder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //only add secrets in development
            if (isDevelopment)
            {
                builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();

            if (isDevelopment)
            {
                Environment.SetEnvironmentVariable("TOKEN", Configuration["TOKEN"]);
            }

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("TOKEN"),
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                AutoReconnect=true,
                
            });

            var commands = Discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { ">" }
            });

            commands.RegisterCommands<MyFirstModule>();
            commands.RegisterCommands<MusicModule>();
            commands.RegisterCommands<BobaModule>();
            commands.RegisterCommands<EggModule>();

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "newmydiscordbot-lavalink.herokuapp.com", // From your server configuration.
                //Port = Convert.ToInt32(Environment.GetEnvironmentVariable("PORT") ?? "80")  // From your server configuration
                //Hostname = "127.0.0.1",
                Port = 80
            };

            Console.WriteLine(endpoint.Port);

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "lavalinkpass", // From your server configuration.
                //Password= "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint,
                ResumeTimeout = 30
            };

            var lavalink = Discord.UseLavalink();

            await Discord.ConnectAsync(new DSharpPlus.Entities.DiscordActivity("Anime",DSharpPlus.Entities.ActivityType.Watching));
            await lavalink.ConnectAsync(lavalinkConfig);
            await ConnectToLavalinkAsync(lavalink, lavalinkConfig);
            await Task.Delay(-1);
        }

        public static async Task ConnectToLavalinkAsync(LavalinkExtension lavalink, LavalinkConfiguration config)
        {
            await Task.Delay(TimeSpan.FromSeconds(55)).ConfigureAwait(false);

            await lavalink.ConnectAsync(config);

            await ConnectToLavalinkAsync(lavalink, config);
        }
    }
}
