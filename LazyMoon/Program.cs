using Blazored.LocalStorage;
using LazyMoon.Model;
using LazyMoon.Service;
using LazyMoon.Service.DBService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using System.Configuration;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 102400000;
});
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

#region DataBase   
builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.AddDBService();
#endregion

builder.Services.AddBlazoredLocalStorage();

#region Twitch Api
string oAuth = builder.Configuration.GetValue<string>("TwitchOAuth:OAuth");
string clientId = builder.Configuration.GetValue<string>("TwitchOAuth:ClientId");
string clientSecret = builder.Configuration.GetValue<string>("TwitchOAuth:ClientSecret");
Encryption.Encryption.SetDefaultPassword(clientSecret);
#endregion
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddSingleton<ServerCounterService>();
builder.Services.AddTransient<LazyMoon.Service.BlazorTimerService>();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();


var app = builder.Build();

StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, builder.Configuration);

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
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


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
