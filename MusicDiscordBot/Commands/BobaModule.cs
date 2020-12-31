using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicDiscordBot.Commands
{
    public class BobaModule : BaseCommandModule
    {
        [Command("бобаметр")]
        [Aliases("пушкаметр")]
        public async Task Bobametr(CommandContext ctx)
        {
            var rng = new Random();

            var boba = rng.Next(0, 41);

            var builder = new DiscordEmbedBuilder()
                .WithTitle("Твій боба має:")
                .WithDescription($"{boba}см");

            if (boba > 20)
            {
                builder.WithImageUrl("https://media.giphy.com/media/5VKbvrjxpVJCM/giphy.gif");
            }
            else if (boba < 10)
            {
                builder.WithImageUrl("https://media.giphy.com/media/7J4Lvpz55rocVYccdH/giphy.gif");
            }
            else
            {
                builder.WithImageUrl("https://media.giphy.com/media/3o7abKhOpu0NwenH3O/giphy.gif");
            }

            await ctx.RespondAsync("",false,builder.Build());
        }
    }
}
