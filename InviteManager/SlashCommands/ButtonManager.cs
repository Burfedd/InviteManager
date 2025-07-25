using Discord;
using Discord.WebSocket;
using InviteManager.Entities;
using InviteManager.Utility;

namespace InviteManager.SlashCommands
{
    public interface IButtonManager
    {
        Task HandleAcceptButton(SocketMessageComponent component);
        Task HandleRescheduleButton(SocketMessageComponent component);
    }

    public class ButtonManager : IButtonManager
    {
        private readonly IInvitationContext _invitationContext;

        public ButtonManager(IInvitationContext invitationContext)
        {
            _invitationContext = invitationContext;
        }

        public async Task HandleAcceptButton(SocketMessageComponent component)
        {
            lock ( _invitationContext )
            {
                var messageId = component.Message.Interaction.Id;
                Invitation currentInvitation = null;
                if ( _invitationContext.Invitations.TryGetValue(messageId, out currentInvitation) )
                {
                    if ( string.IsNullOrEmpty(currentInvitation.Message) )
                    {
                        currentInvitation.Message = component.Message.Content;
                    }
                    var currentUserId = component.User.Id;

                    if ( !currentInvitation.UsersReacted.Contains(currentUserId) )
                    {
                        currentInvitation.UsersReacted.Add(currentUserId);
                        if ( !currentInvitation.IsFull )
                        {
                            currentInvitation.Count++;
                            var newMessage = $"{currentInvitation.Message}\n > {new Emoji(Constants.Emojis.WhiteCheckmark)} [{currentInvitation.Count}/{currentInvitation.MaximumCapacity}] {component.User.GlobalName} ({component.User.Username}) has reserved a slot at {component.CreatedAt.ToLocalTime().ToString("HH:mm")}";
                            currentInvitation.Message = newMessage;
                            component.UpdateAsync(message => message.Content = newMessage);
                        }
                        else
                        {
                            var newMessage = $"{currentInvitation.Message}\n > {new Emoji(Constants.Emojis.Crossmark)} {component.User.GlobalName} ({component.User.Username}) tried joining but the session was full";
                            currentInvitation.Message = newMessage;
                            component.UpdateAsync(message => message.Content = newMessage);
                        }
                    }
                    else
                    {
                        component.RespondAsync(text: "Sorry, but you have already reacted to this invite", ephemeral: true);
                    }
                }
            }
        }

        public async Task HandleRescheduleButton(SocketMessageComponent component)
        {
            lock ( _invitationContext )
            {
                var messageId = component.Message.Interaction.Id;
                if ( _invitationContext.Invitations.TryGetValue(messageId, out Invitation currentInvitation) )
                {
                    var currentUserId = component.User.Id;

                    if ( !currentInvitation.UsersReacted.Contains(currentUserId) )
                    {
                        var newMessage = $"{currentInvitation.Message}\n > {new Emoji(Constants.Emojis.Timer)} {component.User.GlobalName} ({component.User.Username}) suggests rescheduling to a different time";
                        currentInvitation.Message = newMessage;
                        currentInvitation.UsersReacted.Add(currentUserId);
                        component.UpdateAsync(message => message.Content = $"{component.Message.Content}\n > {new Emoji(Constants.Emojis.Timer)} {component.User.GlobalName} ({component.User.Username}) suggests rescheduling to a different time");
                    }
                    else
                    {
                        component.RespondAsync(text: "Sorry, but you have already reacted to this invite", ephemeral: true);
                    }
                }
            }
        }
    }
}
