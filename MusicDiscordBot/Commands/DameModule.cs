using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicDiscordBot.Commands
{
    public class DameModule : BaseCommandModule
    {
        [Command("даме")]
        [Aliases("дамедаме", "даме_даме", "dame", "damedame")]
        public async Task Dame(CommandContext ctx)
        {
            var builder = new DiscordEmbedBuilder();

            builder.WithImageUrl("https://cdn.discordapp.com/attachments/649977684973060136/789525283472277504/generated.gif");

            await ctx.RespondAsync("", false, builder.Build());
        }
    }
}
