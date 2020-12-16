using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MasterYoda.Modules
{
    public class Moderation :  ModuleBase
    {
        

        public bool IsVerified()
        {
            //var role = (Context.Channel as SocketGuildChannel).Guild.Roles.FirstOrDefault(x => x.Id == 788501697357545554);

            var roles = (Context.User as SocketGuildUser).Roles.ToArray();
            for(int i = 0; i < roles.Length; i++)
            {
                if (roles[i].Id == 788501697357545554)
                    return true;
            }
            Context.Channel.SendMessageAsync("Du dich noch nicht verifiziert hast");
            return false;
        }

        [Command("Clear")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {

            //if (!IsVerified()) return;

            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var message = await Context.Channel.SendMessageAsync($"{messages.Count()} Nachrichten erfolgreich gelöscht ich habe!");
            await Task.Delay(2500);
            await message.DeleteAsync();
        }

        [Command("Clear")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(string param = "all")
        {
            //if (!IsVerified()) return;
            if (param.ToLower().Equals("all"))
            {
                var messages = await Context.Channel.GetMessagesAsync(100).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

                var message = await Context.Channel.SendMessageAsync($"{messages.Count()} Nachrichten erfolgreich gelöscht ich habe!");
                await Task.Delay(2500);
                await message.DeleteAsync();
            }
            else
                await Context.Channel.SendMessageAsync($"Unerlaubtes Attribut '{param}' verwendet du hast");

        }

        

    }
}
