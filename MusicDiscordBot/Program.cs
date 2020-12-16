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
                MinimumLogLevel = LogLevel.Debug
            });

            var commands = Discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { ">" }
            });

            commands.RegisterCommands<MyFirstModule>();
            commands.RegisterCommands<MusicModule>();
            commands.RegisterCommands<BobaModule>();

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "mymusicbot-lavalink.herokuapp.com", // From your server configuration.
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("PORT") ?? "80")  // From your server configuration
            };

            Console.WriteLine(endpoint.Port);

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "lavalinkpass", // From your server configuration.
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = Discord.UseLavalink();


            await Discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }
    }
}
