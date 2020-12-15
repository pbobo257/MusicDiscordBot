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
        public async Task Bobametr(CommandContext ctx)
        {
            var rng = new Random();
            

            var builder = new DiscordEmbedBuilder()
                .WithTitle("Твій боба має:")
                .WithDescription($"{rng.Next(0, 40)}см");

            await ctx.RespondAsync("",false,builder.Build());
        }
    }
}
