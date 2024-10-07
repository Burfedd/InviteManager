using System.ComponentModel.DataAnnotations;

namespace InviteManager.Utility.Configuration
{
    public interface IConfigurationProvider
    {
        [Required]
        public string Token { get; }

        [Required]
        public ulong GuildId { get; }
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        public string Token => Environment.GetEnvironmentVariable(Constants.VariableNames.Token) ?? string.Empty;

        public ulong GuildId => ulong.Parse(Environment.GetEnvironmentVariable(Constants.VariableNames.GuildId));
    }
}
