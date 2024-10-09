using Discord;
using Discord.WebSocket;
using InviteManager.SlashCommands;
using InviteManager.Utility.Configuration;
using Microsoft.Extensions.Hosting;

namespace InviteManager
{
    public class Bot : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfigurationProvider _config;
        private readonly ISlashCommandManager _manager;
        private readonly IButtonManager _buttonManager;

        public Bot(
            DiscordSocketClient client,
            IConfigurationProvider config,
            ISlashCommandManager manager,
            IButtonManager buttonManager
            )
        {
            _client = client;
            _config = config;
            _manager = manager;
            _buttonManager = buttonManager;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _client.Ready += OnReady;
            _client.SlashCommandExecuted += OnSlashCommandExecuted;
            _client.ButtonExecuted += OnButtonExecuted;
            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
        }

        private async Task OnReady()
        {
            try
            {
                var guild = _client.GetGuild(_config.GuildId);
                var guildCommand = new SlashCommandBuilder();
                guildCommand.WithName("invite");
                guildCommand.WithDescription("Create an interactive invitation");
                guildCommand.AddOption("game", ApplicationCommandOptionType.String, "Game to be played", isRequired: true);
                guildCommand.AddOption("time", ApplicationCommandOptionType.String, "Scheduled time for the game session", isRequired: true);
                guildCommand.AddOption("max", ApplicationCommandOptionType.Integer, "Maximum number of participants", isRequired: false);
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }

        private async Task OnSlashCommandExecuted(SocketSlashCommand command)
        {
            switch (command.CommandName)
            {
                case "invite":
                    await _manager.HandleInviteCommand(command);
                    break;
                default:
                    break;
            }
        }

        private async Task OnButtonExecuted(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "btn-accept":
                    await _buttonManager.HandleAcceptButton(component);
                    break;
                case "btn-suggest":
                    await _buttonManager.HandleRescheduleButton(component);
                    break;
                default:
                    break;
            }
        }
    }
}
