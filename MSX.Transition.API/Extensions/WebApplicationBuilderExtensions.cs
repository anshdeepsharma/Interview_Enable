using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using MSX.Common.Infra.Utils;
using System.Diagnostics.CodeAnalysis;

namespace MSX.Transition.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class WebApplicationBuilderExtensions
    {
        public static void AddAzureAppConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                var appCSEndpoint = Environment.GetEnvironmentVariable("APP_CS_ENDPOINT");
                ArgumentNullException.ThrowIfNull(appCSEndpoint, "EnvironmentVariable: APP_CS_ENDPOINT");

                var appCSCacheExpiryInMinsAsString = Environment.GetEnvironmentVariable("APP_CS_CACHE_EXP_IN_MINS");
                if (!int.TryParse(appCSCacheExpiryInMinsAsString, out int appCSCacheExpiryInMins))
                {
                    appCSCacheExpiryInMins = 1;
                }

                var appCSFeatureFlagCacheExpiryInMinsAsString = Environment.GetEnvironmentVariable("APP_CS_FF_CACHE_EXP_IN_MINS");
                if (!int.TryParse(appCSFeatureFlagCacheExpiryInMinsAsString, out int appCSFeatureFlagCacheExpiryInMins))
                {
                    appCSFeatureFlagCacheExpiryInMins = 1;
                }

                var azAppCSOptions = options.Connect(new Uri(appCSEndpoint!), CredentialHelper.GetTokenCredential(builder.Environment))
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("BuildId", refreshAll: true).SetCacheExpiration(new TimeSpan(0, appCSCacheExpiryInMins, 0));
                })
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(CredentialHelper.GetTokenCredential(builder.Environment));
                })
                .Select(KeyFilter.Any, LabelFilter.Null);

            });

            builder.Services.AddAzureAppConfiguration();
        }
    }
}
