namespace InviteManager.Entities
{
    public interface IInvitationContext
    {
        Dictionary<ulong, Invitation> Invitations { get; set; }

        Task AddInvitation(ulong messageId, long maximumCapacity, string game, string schedule);
    }

    public class InvitationContext : IInvitationContext
    {
        public Dictionary<ulong, Invitation> Invitations { get; set; }

        public InvitationContext()
        {
            Invitations = new Dictionary<ulong, Invitation>();
        }

        public async Task AddInvitation(ulong messageId, long maximumCapacity, string game, string schedule)
        {
            var invitation = new Invitation
            {
                MaximumCapacity = maximumCapacity,
                Count = 0,
                Game = game,
                ScheduledTime = schedule,
            };
            Invitations.Add(messageId, invitation);
        }
    }
}
