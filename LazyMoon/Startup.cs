using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LazyMoon.Class;
using LazyMoon.Server.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Net.Http;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.ResponseCompression;
using LazyMoon.Model;
using LazyMoon.Service;
using TwitchLib.Api;
using MudBlazor.Services;
using LazyMoon.Service.DBService;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using WebDav;

namespace LazyMoon
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR(e =>
            {
                e.MaximumReceiveMessageSize = 102400000;
            });
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            #region DataBase
            services.AddDbContextFactory<AppDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DBConnection")));
            services.AddDBService();
            #endregion

            services.AddBlazoredLocalStorage();

            #region Twitch Api
            string oAuth = Configuration.GetValue<string>("TwitchOAuth:OAuth");
            string clientId = Configuration.GetValue<string>("TwitchOAuth:ClientId");
            string clientSecret = Configuration.GetValue<string>("TwitchOAuth:ClientSecret");
            Encryption.Encryption.SetDefaultPassword(clientSecret);

            services.AddTransient<TwitchAPI>();
            #endregion
            services.AddTransient<TwitchService>();
            services.AddScoped<ClipboardService>();
            services.AddSingleton<ServerCounterService>();
            services.AddTransient<LazyMoon.Service.BlazorTimerService>();
            services.AddMudServices();
            services.AddTransient<TwitchBotService>();
            services.AddTransient<TTSService>();
            services.AddTransient<ValorantRankService>();
            services.AddTransient<TextToImage>();        
            services.AddSingleton<Abot>();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            StaticWebAssetsLoader.UseStaticWebAssets(env, Configuration);

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            //

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}