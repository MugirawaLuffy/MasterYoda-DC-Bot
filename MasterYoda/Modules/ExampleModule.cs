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
        
        private readonly ILogger<ExampleModule> _logger;
        private readonly Images _images;

        public ExampleModule(ILogger<ExampleModule> logger, Images images)
        {
            _logger = logger;
            
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

        

        [Command("Image", RunMode = RunMode.Async)]
        public async Task Image(SocketGuildUser user)
        {
            await Context.Channel.TriggerTypingAsync();

            var path = await _images.CreateImageAsync(user);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }
    }
}