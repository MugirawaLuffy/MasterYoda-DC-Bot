using System.Data;
using System.IO;
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
        private readonly Servers _servers;
        private readonly ILogger<ExampleModule> _logger;
        private readonly Images _images;

        public ExampleModule(ILogger<ExampleModule> logger, Servers servers, Images images)
        {
            _logger = logger;
            _servers = servers;
            _images = images;
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

        [Command("prefix")]
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

        [Command("Image", RunMode = RunMode.Async)]
        public async Task Image(SocketGuildUser user)
        {
            var path = await _images.CreateImageAsync(user);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }
    }
}