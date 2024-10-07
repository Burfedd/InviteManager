using Discord;

namespace InviteManager.Utility.Logging
{
    public class BotLogger
    {
        public static Task Log(LogMessage msg)
        {
            Console.WriteLine($"[BotLogger] {msg.Message}");
            return Task.CompletedTask;
        }
    }
}
