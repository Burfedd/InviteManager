namespace InviteManager.Entities
{
    public class Invitation
    {
        public Invitation()
        {
            UsersReacted = new List<ulong>();
        }

        public long MaximumCapacity { get; set; }
        public string Game { get; set; }
        public string ScheduledTime { get; set; }
        public long Count { get; set; }
        public bool IsFull => Count >= MaximumCapacity;
        public List<ulong> UsersReacted { get; set; }
        public string Message { get; set; }
    }
}
