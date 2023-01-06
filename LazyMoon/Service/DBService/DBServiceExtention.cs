using Microsoft.Extensions.DependencyInjection;

namespace LazyMoon.Service.DBService
{
    static public class DBServiceExtentions
    {
        public static IServiceCollection AddDBService(this IServiceCollection services)
        {
            services.AddTransient<DBUserService>();
            services.AddTransient<DBTTSService>();
            services.AddTransient<DBValorantRankService>();
            services.AddTransient<DBVoiceService>();
            return services;
        }
    }
}
