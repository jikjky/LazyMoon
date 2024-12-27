using LazyMoon.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace LazyMoon.Class.Service.DBService
{
    static public class DBServiceExtentions
    {
        public static IServiceCollection AddDBService(this IServiceCollection services)
        {
            services.AddTransient<DBUserService>();
            services.AddTransient<DBTTSService>();
            services.AddTransient<DBValorantRankService>();
            services.AddTransient<DBVoiceService>();
            services.AddTransient<DBConnectionHistory>();
            return services;
        }
    }
}
