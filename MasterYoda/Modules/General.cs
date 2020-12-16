using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterYoda.Modules
{
    public class General : ModuleBase
    {
        [Command("hallo")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Gegrüßt du seist!", true);
        }

        [Command("info")]
        public async Task Info(SocketGuildUser user = null)
        {
            if(user == null)
            {
                var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                .WithDescription("In dieser Nachricht Infos über dich selber du findest!")
                .WithColor(new Color(255, 64, 229))
                .AddField("User ID", Context.User.Id, true)
                .AddField("Account Created at", Context.User.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription($"In dieser Nachricht Infos über {user.Username} du findest!")
                .WithColor(new Color(255, 64, 229))
                .AddField("User ID", user.Id, true)
                .AddField("Account Created at", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", (user as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" ", (user as SocketGuildUser).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp();
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }


            
        }

        

        [Command("Server")]
        public async Task Server()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Hier ein paar Infos über den Server du hast!")
                .WithTitle($"{Context.Guild.Name} Übersicht")
                .WithColor(new Color(10, 255, 251))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Membercount", (Context.Guild as SocketGuild).MemberCount + " members", true)
                .AddField("Now active", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count()
                    + " members", true);
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

    }
}
