using Discord.WebSocket;
using InviteManager.Entities;
using InviteManager.SlashCommands;
using InviteManager.Utility.Configuration;
using InviteManager.Utility.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InviteManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            // Add services
            builder.Services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();
            builder.Services.AddSingleton<DiscordSocketClient>(client => 
            {
                var c = new DiscordSocketClient();
                c.Log += BotLogger.Log;
                return c;
            });
            builder.Services.AddSingleton<IInvitationContext, InvitationContext>();
            builder.Services.AddSingleton<ISlashCommandManager, SlashCommandManager>();
            builder.Services.AddSingleton<IButtonManager, ButtonManager>();
            
            // Add bot entry point
            builder.Services.AddHostedService<Bot>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}
