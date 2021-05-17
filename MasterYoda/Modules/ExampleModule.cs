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
using DeepL;
using System.Net;
using System.Web;

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
        public async Task PingAsync(SocketGuildUser user)
        {
            await Context.Channel.TriggerTypingAsync();

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
                "(Die fucking bitch funktioniert noch nicht)")
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
        public async Task Rank([Remainder] string identifier)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);

            IRole role;

            if (ulong.TryParse(identifier, out ulong roleId))
            {
                var roleById = Context.Guild.Roles.FirstOrDefault(x => x.Id == roleId);
                if (roleById == null)
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
            if (ranks.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("Diesen Rang es nicht gibt!");
                return;
            }
            if ((Context.User as SocketGuildUser).Roles.Any(x => x.Id == role.Id))
            {
                await (Context.User as SocketGuildUser).RemoveRoleAsync(role);
                await ReplyAsync($"Erfolgreich den Rang `{role.Mention}` dir genommen ich habe!");
                return;
            }

            await (Context.User as SocketGuildUser).AddRoleAsync(role);
            await ReplyAsync($"Erfolgreich den Rang `{role.Mention}` dir gegeben ich habe!");
        }

        [Command("Translate", RunMode = RunMode.Async)]
        public async Task Translate([Remainder]string input)
        {
            await Context.Channel.TriggerTypingAsync();

            string responseTitle = string.Empty;
            string response = string.Empty;
            string detectedLang = string.Empty;
            bool success = false;

            string[] seperatedInput = input.Split(" ");
            string targetLang = seperatedInput[0];
            string request = string.Empty;
            for (int i = 1; i < seperatedInput.Length; i++)
            {
                request += seperatedInput[i];
                request += " ";
            }

            
            try
            {
                var toLanguage = "";
                var fromLanguage = "";

                if (targetLang == "D-S")
                {
                    toLanguage = "es";
                    fromLanguage = "de";             
                }
                else if (targetLang == "S-D")
                {
                    toLanguage = "de";
                    fromLanguage = "es"; 
                }
                else if (targetLang == "E-D")
                {
                    toLanguage = "de";
                    fromLanguage = "en";
                }
                else if (targetLang == "D-E")
                {
                    toLanguage = "en";
                    fromLanguage = "de";
                }
                else if (targetLang == "S-E")
                {
                    toLanguage = "en";
                    fromLanguage = "es";
                }
                else if (targetLang == "E-S")
                {
                    toLanguage = "es";
                    fromLanguage = "en";
                }else
                {
                    throw new Exception("Kein gültiges Sprachpaar erkannt ich habe!");
                }

                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(request)}";
                var webClient = new WebClient
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                var result = webClient.DownloadString(url);
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                response = result;

                success = true;

            }
            catch (Exception e)
            {
                responseTitle = "Schief gelaufen etwas ist.";
                response = e.Message;
                success = false;
            }

            if (success)
            {
                var builder = new EmbedBuilder()
                .WithColor(new Color(255, 251, 10))
                .WithTitle($"Übersetzung")
                .WithDescription(response)
                .WithFooter(detectedLang);
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync("Deine Übersetzung fertig ist!", false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                .WithColor(new Color(255, 10, 10))
                .WithTitle(responseTitle)
                .WithDescription(response)
                .WithFooter("(Vielleicht du etwas falsch gemacht hast. Wahrscheinlich einfach kacke der Code ist!)");
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync("Deine Übersetzung auf dem Weg ist!", false, embed);
            }
            
        }
    


        /*
        [Command("Translate")]
        [Alias("")]
        public async Task TranlsateDS([Remainder] string input)
        {
            string responseTitle = string.Empty;
            string response = string.Empty;
            string detectedLang = string.Empty;
            bool success = false;

            string[] seperatedInput = input.Split(" ");
            string targetLang = seperatedInput[0];
            string request = string.Empty;
            for (int i = 1; i < seperatedInput.Length; i++)
            {
                request += seperatedInput[i];
                request += " ";
            }

            using (DeepLClient client = new DeepLClient("6b39626d-2f51-2da6-3bab-7dfaa482f31a%3Afx", useFreeApi: true))
            {
                try
                {
                    if (targetLang == "esp")
                    {
                        Translation t = await client.TranslateAsync(
                            request,
                            Language.Spanish);
                        detectedLang = t.DetectedSourceLanguage;
                        response = t.Text;
                    }
                    if (targetLang == "eng")
                    {
                        Translation t = await client.TranslateAsync(
                            request,
                            Language.English);
                        detectedLang = t.DetectedSourceLanguage;
                        response = t.Text;
                    }
                    if (targetLang == "ger")
                    {
                        Translation t = await client.TranslateAsync(
                            request,
                            Language.German);
                        detectedLang = t.DetectedSourceLanguage;
                        response = t.Text;
                    }

                    success = true;

                }
                catch (DeepLException e)
                {
                    responseTitle = "Oops, something went wrong";
                    response = e.Message;
                    success = false;
                }

                if (success)
                {
                    var builder = new EmbedBuilder()
                    .WithColor(new Color(255, 251, 10))
                    .WithTitle($"Translation")
                    .WithDescription(response)
                    .WithFooter(detectedLang);
                    var embed = builder.Build();
                    await Context.Channel.SendMessageAsync("Deine Übersetzung fertig ist!", false, embed);
                }
                else
                {
                    var builder = new EmbedBuilder()
                    .WithColor(new Color(255, 10, 10))
                    .WithTitle(responseTitle)
                    .WithDescription(response)
                    .WithFooter("(Maybe you failed, probably my code sucks)");
                    var embed = builder.Build();
                    await Context.Channel.SendMessageAsync("Ein Meme ich dir jetzt schicke", false, embed);
                }
            }
        }
        */
    }
}