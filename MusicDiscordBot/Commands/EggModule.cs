using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicDiscordBot.Commands
{
    public class EggModule : BaseCommandModule
    {
        private readonly Random rng = new Random();

        [Command("хто_я?")]
        public async Task EggCounter(CommandContext ctx)
        {

            var eggs = rng.Next(1, 4);

            var builder = new DiscordEmbedBuilder();

            if (eggs == 1)
            {
                builder.WithTitle("Ти кастрований кіт");
            }
            else if (eggs == 2)
            {
                builder.WithTitle("Ти знаєш що ти людина?");
            }
            else
            {
                builder.WithTitle("Ото ти Чингісхан!");
            }

            await ctx.RespondAsync("", false, builder.Build());
        }
    }
}
