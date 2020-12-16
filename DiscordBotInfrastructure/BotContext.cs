using Microsoft.EntityFrameworkCore;
using System;

namespace DiscordBotInfrastructure
{
    public class BotContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("server=localhost;user=root;database=yodadb;port=3306;Connect Timeout=5;");

        
        
    }
    public class Server
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }

    }
}
