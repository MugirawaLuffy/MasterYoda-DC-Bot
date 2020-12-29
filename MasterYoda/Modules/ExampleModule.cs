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
        [Command("ping")]
        public async Task PingAsync(SocketGuildUser user)
        {
            Context.Channel.TriggerTypingAsync();

            var builder = new EmbedBuilder()
                .WithColor(new Color(255, 251, 10))
                .WithTitle("Klick mich")
                .WithUrl("https://tenor.com/view/rickroll-dance-funny-you-music-gif-7755460")
                .WithFooter("Tu es");
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync($"@{user.Username}", false, embed);
            //await Context.Channel.SendMessageAsync();
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

        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.TriggerTypingAsync();

            var builder = new EmbedBuilder()
                .WithColor(new Color(255, 251, 10))
                .WithTitle("Commands")
                .AddField("Images", "Benutzt die superlichtgeschwindigkeitschnelle GTX1080 um ein Bild mit DIR zu getalten")
                .AddField("role <Rolle>", "gibt/ nimmt dir eine bestimmte Rolle")
                .AddField("prefix", "Holt dir das momentane Prefix aus der Datenbank")
                .AddField("prefix <NeuesPrefix>", "das erklär ich nich")
                .AddField("autoroles", "zeigt dir alle rollen, die zu den Autoroles gehören. \n Autoroles werden " +
                "neuen benutzern Automatisch hinzugefügt")
                .AddField("addautorole / delautorole", "logisch")
                .AddField("Join", "sorgt dafür, dass der Bot in deinen Channel kommt")
                .AddField("Play <Suchanfrage>", "Stellt eine REST-Anfrage an die Youtube API und spielt das lied ab" +
                "(Die fucking bitch funktioniert noch nicht)" )
                .AddField("Meme", "holt dir ein zufälliges Meme mithilfe der Reddit API")
                .AddField("Reddit <subreddit>", "durchsucht ein Subreddit")
                .AddField("r6-randomizer <attacker/defender>", "gibt dir ein random layout für r6")
                .AddField("Clear <amount>", "löscht <amount> Nachrichten im Chat")
                .AddField("help", "gibt dir hylfe")
                .WithFooter("Wenn das nicht hylft, gerne den Meister Programmierer @luffy contacten, boyy");
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync("", false, embed);
            await ReplyAsync("Neue Befehlvorschläge hier äußern!");
        }



        [Command("Image", RunMode = RunMode.Async)]
        public async Task Image(SocketGuildUser user)
        {
            await Context.Channel.TriggerTypingAsync();

            var path = await _images.CreateImageAsync(user);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }
        [Command("Image", RunMode = RunMode.Async)]
        public async Task Image()
        {
            SocketGuildUser user = (Context.User as SocketGuildUser);
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