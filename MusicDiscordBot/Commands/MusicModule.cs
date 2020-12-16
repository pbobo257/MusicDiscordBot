using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicDiscordBot.Commands
{
    class MusicModule : BaseCommandModule
    {
        Queue<LavalinkTrack> Queue = new Queue<LavalinkTrack>();
        int PrevQueueCount = 0;
        CommandContext SoundContext;

        [Command("queue")]
        public async Task ShowQueue(CommandContext ctx)
        {
            if (Queue.Count == 0)
            {
                await ctx.RespondAsync("There are no songs in queue");
                return;
            }
            //var tempQueue = Queue;
            StringBuilder sb = new StringBuilder();
            foreach(var track in Queue)
            {
                sb.AppendLine($"-{track.Title}");
            }

            var builder = new DiscordEmbedBuilder()
                .WithTitle("Current queue:")
                .WithDescription(sb.ToString());

            await ctx.RespondAsync(null, false, builder.Build());
        }

        [Command]
        public async Task Leave(CommandContext ctx)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            Queue = new Queue<LavalinkTrack>();
            await ctx.RespondAsync($"Left {ctx.Member.VoiceState.Channel.Name}!");
        }

        [Command]
        public async Task Skip(CommandContext ctx)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            var currentTrack = conn.CurrentState.CurrentTrack;
            if(currentTrack == null)
            {
                await ctx.RespondAsync("Nothing to skip.");
                return;
            }

            await conn.SeekAsync(currentTrack.Length);
        }

        [Command]
        public async Task Play(CommandContext ctx, Uri url)
        {
            var (node, conn) = await ConnectToChannel(ctx);

            var loadResult = await node.Rest.GetTracksAsync(url);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {url}.");
                return;
            }

            if(loadResult.LoadResultType == LavalinkLoadResultType.PlaylistLoaded)
            {
                foreach (var tr in loadResult.Tracks)
                    Queue.Enqueue(tr);
            }
            else
            {
                Queue.Enqueue(loadResult.Tracks.First());
            }

            SoundContext = ctx;
            await PlayNext(conn, null);
        }
        [Command]
        public async Task Play(CommandContext ctx, [RemainingText] string search)
        {
            var (node, conn) = await ConnectToChannel(ctx);

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}.");
                return;
            }

            Queue.Enqueue(loadResult.Tracks.First());

            SoundContext = ctx;
            await PlayNext(conn, null);
        }

        private async Task<(LavalinkNodeConnection, LavalinkGuildConnection)>  ConnectToChannel(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return (null,null);
            }

            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return (null, null);
            }

            var node = lava.ConnectedNodes.Values.First();

            if (ctx.Member.VoiceState.Channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return (null, null);
            }

            await node.ConnectAsync(ctx.Member.VoiceState.Channel);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return (null, null);
            }

            return (node,conn);
        }

        private async Task PlayNext(LavalinkGuildConnection conn, TrackFinishEventArgs args)
        {
            if (conn.CurrentState.CurrentTrack != null)
            {
                if (PrevQueueCount == 0)
                    conn.PlaybackFinished += PlayNext;

                return;
            }

            var track = Queue.Dequeue();
            await conn.PlayAsync(track);
            if (Queue.Count > 0)
            {
                conn.PlaybackFinished += PlayNext;
            }

            var builder = new DiscordEmbedBuilder()
                .WithTitle("Playing now:")
                .WithDescription($"[{track.Title}]({track.Uri.AbsoluteUri})");

            await SoundContext.RespondAsync(null, false, builder.Build());
        }

        [Command]
        public async Task Pause(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }

            await conn.PauseAsync();
        }

        [Command]
        public async Task Resume(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }

            await conn.ResumeAsync();
        }

    }
}
