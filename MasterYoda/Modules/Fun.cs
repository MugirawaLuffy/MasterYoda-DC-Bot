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
            //if (!Moderation.IsVerified()) return;

            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "memes"}/random.json?limit=1");
            if(!result.StartsWith("["))
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
    }
}
