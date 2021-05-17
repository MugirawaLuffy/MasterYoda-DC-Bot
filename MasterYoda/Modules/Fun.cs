using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;



namespace MasterYoda.Modules
{
    public class Fun : ModuleBase
    {
        [Command("meme")]
        [Alias("reddit")]
        public async Task Meme(string subreddit = null)
        {
            await Context.Channel.TriggerTypingAsync();
            //if (!Moderation.IsVerified()) return;

            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "memes"}/random.json?limit=1");
            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("Das subreddit es nicht gibt!");
                return;
            }

            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(new Color(255, 251, 10))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"].ToString())
                .WithFooter($":speech_left: {post["num_comments"]} :arrow_up: {post["ups"]}");
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync("Ein Meme ich dir jetzt schicke", false, embed);
        }

        [Command("r6-randomizer")]
        public async Task RainbowRand([Remainder] string arg)
        {
            await Context.Channel.TriggerTypingAsync();


            string layout = string.Empty;
            var rainbow = new RainbowSixChallenges.RandomClient();

            if (arg.Equals("attacker"))
            {
                layout = rainbow.GetRandomAttacker();
            }
            else if (arg.Equals("defender"))
            {
                layout = rainbow.GetRandomDefender();
            }

            var builder = new EmbedBuilder()
                .WithColor(new Color(255, 251, 10))
                .WithTitle("Random " + arg + " layout")
                .AddField("layout", layout)
                .WithFooter("Falls das Scope für die Waffe nicht verfügbar ist, das nächst schlechtere nehmen du musst!");
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync($"Ein Random `{arg}` layout die Macht dir gibt", false, embed);
        }

        [Command("ow-randomizer")]
        public async Task OverwatchRand()
        {
            await Context.Channel.TriggerTypingAsync();

            RainbowSixChallenges.OverwatchRandomizer rand = new RainbowSixChallenges.OverwatchRandomizer();
            string op = rand.GetRandomOperator();

            var builder = new EmbedBuilder()
                .WithColor(new Color(250, 250, 250))
                .WithTitle("Random " + "Overwatch " + "Operator")
                .AddField("Operator", op)
                .WithFooter("Falls der Operator gewählt bereits ist, den am nächsten daneben liegenden nehmen du musst");
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync($"Ein zufälligen Operator die Macht dir gibt", false, embed);
        }
    }
}
