using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotInfrastructure;
using MasterYoda.Utilities;
using Microsoft.Extensions.Logging;

namespace Template.Modules
{
    public class ExampleModule : ModuleBase<SocketCommandContext>
    {
        
        private readonly ILogger<ExampleModule> _logger;
        private readonly Images _images;
        private readonly RanksHelper _ranksHelper;

        public ExampleModule(ILogger<ExampleModule> logger, Images images, RanksHelper ranksHelper)
        {
            _logger = logger;
            
            _images = images;
            _ranksHelper = ranksHelper;
        }
            

        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
            //_logger.LogInformation($"{Context.User.Username} executed the ping command!");
        }

        [Command("echo")]
        public async Task EchoAsync([Remainder] string text)
        {
            await ReplyAsync(text);
            //_logger.LogInformation($"{Context.User.Username} executed the echo command!");
        }

        [Command("math")]
        public async Task MathAsync([Remainder] string math)
        {
            var dt = new DataTable();
            var result = dt.Compute(math, null);
            
            await ReplyAsync($"{result} das Ergebnis ist!");
            //_logger.LogInformation($"{Context.User.Username} executed the math command!");
        }

        [Command("SetupVerifyText")]
        public async Task SetupVerifyText()
        {
            await ReplyAsync("Auf diese Nachricht mit ☃️ reagieren du musst, damit mehr Befehle benutzen du kannst");
        }

        

        [Command("Image", RunMode = RunMode.Async)]
        public async Task Image(SocketGuildUser user)
        {
            await Context.Channel.TriggerTypingAsync();

            var path = await _images.CreateImageAsync(user);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }

        [Command("rank", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Rank([Remainder]string identifier)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);

            IRole role;

            if(ulong.TryParse(identifier, out ulong roleId))
            {
                var roleById = Context.Guild.Roles.FirstOrDefault(x => x.Id == roleId);
                if(roleById == null)
                {
                    await ReplyAsync("Diese Rolle es garnicht gibt!");
                    return;
                }

                role = roleById;
            }
            else
            {
                var roleByName = Context.Guild.Roles
                        .FirstOrDefault(x => string.Equals(x.Name, identifier, StringComparison.CurrentCultureIgnoreCase));
                if (roleByName == null)
                {
                    await ReplyAsync("Diese Rolle es garnicht gibt!");
                    return;
                }
                role = roleByName;
            }
            if(ranks.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("Diesen Rang es nicht gibt!");
                return;
            }
            if((Context.User as SocketGuildUser).Roles.Any(x => x.Id == role.Id))
            {
                await (Context.User as SocketGuildUser).RemoveRoleAsync(role);
                await ReplyAsync($"Erfolgreich den Rang `{role.Mention}` dir genommen ich habe!");
                return;
            }

            await (Context.User as SocketGuildUser).AddRoleAsync(role);
            await ReplyAsync($"Erfolgreich den Rang `{role.Mention}` dir gegeben ich habe!");
        }
    }
}