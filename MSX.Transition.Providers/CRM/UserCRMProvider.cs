using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models;
using MSX.Transition.Providers.Contracts.v1.Provider;
using System.Text.Json;

namespace MSX.Transition.Providers.CRM
{
    public class UserCRMProvider : IUserCRMProvider
    {
        private readonly ILogger<IUserCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;

        private static FieldMapper? UserFieldMapper = JsonSerializer.Deserialize<FieldMapper>(XMLHandler.ReadXMLFile(Constants.MapperPath + Constants.UserFieldMapperFile));

        public UserCRMProvider(ILogger<IUserCRMProvider> logger
            , ICRMProvider crmProvider)
        {
            _logger = logger;
            _crmProvider = crmProvider;
        }

        public async Task<User> ResolveUserAsync(string userAlias)
        {
            User user = new() { Alias = userAlias };

            if (string.IsNullOrEmpty(userAlias))
                return user;

            user.Alias = GetUserDomainName(userAlias);

            var cacheKey = $"PrimaryAlias:{userAlias}";

            var resolvedId = await _crmProvider.ResolveObjectIdAsync(user, cacheKey, UserFieldMapper);

            user.Id = resolvedId?.ToString();
            return user;
        }

        private string? GetUserDomainName(string? userAlias)
        {
            if (string.IsNullOrEmpty(userAlias))
                return userAlias;

            if (!userAlias.EndsWith(Constants.DomainName, StringComparison.InvariantCultureIgnoreCase))
                return string.Concat(userAlias, Constants.DomainName);

            return userAlias;
        }
    }
}