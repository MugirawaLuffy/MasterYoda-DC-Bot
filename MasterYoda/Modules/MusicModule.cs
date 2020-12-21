using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Victoria;
using Victoria.Enums;

namespace MasterYoda.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;

        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command("Join")]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("Bereits in einem VoiceChannel ich bin!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("Mit einem Sprahkanal verbunden du sein musst!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"{voiceState.VoiceChannel.Name} beigetreten ich bin!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }


        //[Command("Play")]
        //public async Task PlayAsync([Remainder] string query)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //    {
        //        await ReplyAsync("Eine Suchanfrage mir geben du musst");
        //        return;
        //    }

        //    if (!_lavaNode.HasPlayer(Context.Guild))
        //    {
        //        await ReplyAsync("Mit keinem Sprachkanal verbunden ich bin. Die Macht benutzen du musst um mich mit join zu holen!");
        //        return;
        //    }




        //    var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
        //    if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
        //        searchResponse.LoadStatus == LoadStatus.NoMatches)
        //    {
        //        await ReplyAsync($"Für `{query}` Nichts gefunden ich habe.");
        //        return;
        //    }

        //    var player = _lavaNode.GetPlayer(Context.Guild);

        //    if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
        //    {  
        //        var track = searchResponse.Tracks[0];
        //        player.Queue.Enqueue(track);
        //        await ReplyAsync($"{track.Title} In die Warteschleife getan ich habe");      
        //    }
        //    else
        //    {
        //        var track = searchResponse.Tracks[0];
        //        await player.PlayAsync(track);
        //        await ReplyAsync($"Der Tytel {track.Title} jetzt gerade abgespielt wird");       
        //    }

        //}

        [Command("Play")]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var queries = searchQuery.Split(' ');
            foreach (var query in queries)
            {
                var searchResponse = await _lavaNode.SearchAsync(query);
                if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                    searchResponse.LoadStatus == LoadStatus.NoMatches)
                {
                    await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                    return;
                }

                var player = _lavaNode.GetPlayer(Context.Guild);

                if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                {
                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                    {
                        foreach (var track in searchResponse.Tracks)
                        {
                            player.Queue.Enqueue(track);
                        }

                        await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                    }
                    else
                    {
                        var track = searchResponse.Tracks[0];
                        player.Queue.Enqueue(track);
                        await ReplyAsync($"Enqueued: {track.Title}");
                    }
                }
                else
                {
                    var track = searchResponse.Tracks[0];

                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                    {
                        for (var i = 0; i < searchResponse.Tracks.Count; i++)
                        {
                            if (i == 0)
                            {
                                await player.PlayAsync(track);
                                await ReplyAsync($"Now Playing: {track.Title}");
                            }
                            else
                            {
                                player.Queue.Enqueue(searchResponse.Tracks[i]);
                            }
                        }

                        await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                    }
                    else
                    {
                        await player.PlayAsync(track);
                        await ReplyAsync($"Now Playing: {track.Title}");
                    }
                }
            }
        }
    }
}
