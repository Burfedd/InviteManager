using Discord;
using Discord.WebSocket;
using InviteManager.Entities;
using InviteManager.Utility;

namespace InviteManager.SlashCommands
{
    public interface ISlashCommandManager
    {
        Task HandleInviteCommand(SocketSlashCommand cmd);
    }

    public class SlashCommandManager : ISlashCommandManager
    {

        private readonly IInvitationContext _invitationContext;
        public SlashCommandManager(IInvitationContext invitationContext)
        {
            _invitationContext = invitationContext;
        }

        public async Task HandleInviteCommand(SocketSlashCommand cmd)
        {
            try
            {
                var builder = new ComponentBuilder();
                var game = ( string )cmd.Data.Options.First(option => option.Name == "game").Value;
                var time = ( string )cmd.Data.Options.First(option => option.Name == "time").Value;
                var maxCapacity = cmd.Data.Options.FirstOrDefault(option => option.Name == "max");
                var max = maxCapacity is not null ? ( long )maxCapacity.Value : 5;
                await _invitationContext.AddInvitation(cmd.Id, max, game, time);
                builder.WithButton(label: "I will be there", customId: "btn-accept", style: ButtonStyle.Primary, emote: new Emoji(Constants.Emojis.WhiteCheckmark));
                builder.WithButton(label: "Let's rechedule", customId: "btn-suggest", style: ButtonStyle.Secondary, emote: new Emoji(Constants.Emojis.Timer));
                await cmd.RespondAsync($"# 🇺🇦 {game} | {time} 🇷🇺 \n > {cmd.User.GlobalName} ({cmd.User.Username}) invites everyone to play **{game}** at **{time}**", components: builder.Build());
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
