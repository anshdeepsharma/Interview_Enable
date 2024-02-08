using MSX.Common.Infra;

namespace MSX.Transition.API
{
    public static class Configuration
    {
        public static IServiceCollection SetupConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TransitionConfig>(cfg =>
            {
                cfg.CheckTransitionLastXHours = configuration["Transition:CheckTransitionLastXHours"] ?? "24";
            });

            return services;
        }
    }
}
