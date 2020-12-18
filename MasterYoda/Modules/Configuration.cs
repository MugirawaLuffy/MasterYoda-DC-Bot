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
        private readonly AutoRolesHelper _autoRolesHelper;
        private readonly Servers _servers;
        private readonly Ranks _ranks;
        private readonly AutoRoles _autoRoles;

        public Configuration(RanksHelper ranksHelper, Servers servers, Ranks ranks, AutoRolesHelper autoRolesHelper, AutoRoles autoRoles)
        {
            _ranksHelper = ranksHelper;
            _servers = servers;
            _ranks = ranks;
            _autoRolesHelper = autoRolesHelper;
            _autoRoles = autoRoles;
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

        [Command("delrank", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        public async Task DelRank([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);
            
            var role = Context.Guild.Roles
                .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("Die Rolle existiert nicht, ein Idiot du bist!");
                return;
            }

            if(ranks.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("Diese Rolle ein Rang nichtmal ist! Du noch viel lernen musst.\n Die Macht benutzen du kannst" +
                    ", um die Rolle mit `addrole name` zu den Rollen hinzuzufügen!");
                return;
            }

            await _ranks.RemoveRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Die Rolle `{role.Mention}` ein Rang nicht länger ist!");
        }


        [Command("autoroles", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoles()
        {
            var autoRoles = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);

            if (autoRoles.Count == 0)
            {
                await ReplyAsync("Keine AutoRoles dieser Server hat!");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "Alle verfügbaren AutoRoles diese Nachricht dir auflistet.\nUm eine AutoRole zu löschen, den Namen oder Id der AutoRole benutzen du kannst";

            foreach (var autoRole in autoRoles)
            {
                description += $"\n{autoRole.Mention} ({autoRole.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("addautorole", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        public async Task AddAutoRoles([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            
            var autoRole = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);
            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("Die Rolle existiert nicht, ein Idiot du bist!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Die Macht zu groß für mich ist!\n ->(nicht berechtigt diese Rolle zu geben ich bin :( )");
                return;
            }

            if (autoRole.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("Eine `AutoRole` diese Rolle bereits ist!");
                return;
            }

            await _autoRoles.AddAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Die Rolle `{role.Mention}` erfolgreich zu den `AutoRoles` hinzugefügt ich habe!");
        }

        [Command("delautorole", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        public async Task DelAutoRole([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles
                .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("Die Rolle existiert nicht, ein Idiot du bist!");
                return;
            }

            if (autoRoles.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("Diese Rolle eine `AutoRole` nichtmal ist! Du noch viel lernen musst.\n Die Macht benutzen du kannst" +
                    ", um die Rolle mit `addautorole name` zu den Rollen hinzuzufügen!");
                return;
            }

            await _autoRoles.RemoveAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Die Rolle `{role.Mention}` eine `AutoRole` nicht länger ist!");
        }

    }
}
