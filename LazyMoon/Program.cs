using Blazored.LocalStorage;
using LazyMoon;
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

#region DataBase   
builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.AddDBService();
#endregion

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ClipboardService>();
builder.Services.AddSingleton<ServerCounterService>();
builder.Services.AddTransient<LazyMoon.Service.BlazorTimerService>();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();


var app = builder.Build();

StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, builder.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//
//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//});

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
