using Discord;
using Discord.Commands;
using DiscordBotInfrastructure;
using MasterYoda.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterYoda.Modules
{
    public class Configuration : ModuleBase<SocketCommandContext>
    {
        private readonly RanksHelper _ranksHelper;
        private readonly Servers _servers;
        private readonly Ranks _ranks;

        public Configuration(RanksHelper ranksHelper, Servers servers, Ranks ranks)
        {
            _ranksHelper = ranksHelper;
            _servers = servers;
            _ranks = ranks;
        }

        [Command("prefix", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? "#";
                await Context.Channel.SendMessageAsync($"`{guildPrefix}` Das Momentane Prefix ist! Es für dich ändern Ich kann!\n" +
                    $"Bloß `{guildPrefix}prefix neuesPrefix` eingeben du musst!");
                return;
            }

            if (prefix.Length > 8)
            {
                await ReplyAsync("Zu lang dein angegebenes Prefix ist. Kein Schwein sich das merken kann!");
                return;
            }

            await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);
            await ReplyAsync($"Die Macht genutzt ich habe!\n Das Prefix `{prefix}` jetzt ist! ");
        }



        [Command("ranks", RunMode = RunMode.Async)]
        public async Task Ranks()
        {
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);

            if (ranks.Count == 0)
            {
                await ReplyAsync("Keine Ränge dieser Server hat!");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "Alle verfügbare Ränge diese Nachricht dir auflistet.\nUm einen neuen Rang hinzuzufügen, den Namen oder Id des Ranges benutzen du kannst";

            foreach(var rank in ranks)
            {
                description += $"\n{rank.Mention} ({rank.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("addrank", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        public async Task  AddRank([Remainder]string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);
            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if(role == null)
            {
                await ReplyAsync("Die Rolle existiert nicht, ein Idiot du bist!");
                return;
            }

            if(role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Die Macht zu groß für mich ist!\n ->(nicht berechtigt diese Rolle zu geben ich bin :( )");
                return;
            }

            if(ranks.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("Ein Rang diese Rolle bereits ist!");
                return;
            }

            await _ranks.AddRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Die Rolle `{role.Mention}` erfolgreich hinzugefügt ich habe!");
        }

    }
}
