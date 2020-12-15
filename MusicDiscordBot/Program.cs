using DSharpPlus;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSharpPlus.CommandsNext;
using System.Reflection;
using MusicDiscordBot.Commands;

namespace MusicDiscordBot
{
    class Program
    {
        public static DiscordClient Discord;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "NzAwNzgyNDUxMjc4MDg2MTkw.Xpn8hA.COYYHb9jXcV6NMa8vXOvqO0PAxY",
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
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 5000  // From your server configuration
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass", // From your server configuration.
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
