using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hamsterland.MyAnimeList.Data;
using Hamsterland.MyAnimeList.Services.Accounts;
using Hamsterland.MyAnimeList.Services.Activity;
using Hamsterland.MyAnimeList.Services.MyAnimeList;
using Hamsterland.MyAnimeList.Startup;
using Interactivity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Uchuumaru.Services;

namespace Hamsterland.MyAnimeList
{
    public class Program
    {
        public static async Task Main(string[] args) => await Host.CreateDefaultBuilder()
            .UseSerilog((_, configuration) =>
            {
                configuration
                    // .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Console(theme: SystemConsoleTheme.Literate);
            })
            .ConfigureAppConfiguration((_, builder) =>
            {
                builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true);
            })
            .ConfigureServices((context, collection) =>
            {
                var client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 10000,
                    LogLevel = LogSeverity.Verbose
                });

                var commands = new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Sync,
                    LogLevel = LogSeverity.Verbose,
                    ThrowOnError = true
                });

                collection
                    .AddHttpClient<IMalWebService, MalWebService>(x => x.Timeout = TimeSpan.FromSeconds(3));

                collection
                    .AddHttpClient<IActivityService, ActivityService>(x => x.Timeout = TimeSpan.FromSeconds(3));

                collection
                    .AddMediatR(typeof(Program).Assembly)
                    .AddHostedService<StartupHostedService>()
                    .AddHostedService<DiscordHostedService>()
                    .AddSingleton<InteractivityService>()
                    .AddSingleton<IAccountService, AccountService>()
                    .AddSingleton<IActivityService, ActivityService>()
                    .AddSingleton<IMalWebService, MalWebService>()
                    .AddSingleton(new InteractivityConfig {DefaultTimeout = TimeSpan.FromSeconds(20)})
                    .AddSingleton(client)
                    .AddSingleton(provider =>
                    {
                        commands.AddModulesAsync(typeof(Program).Assembly, provider);
                        return commands;
                    })
                    .AddDbContext<HamsterlandContext>(options =>
                    {
                        options.UseNpgsql(context.Configuration["Postgres:Connection"]);
                    })
                    .AddDbContextFactory<HamsterlandContext>(options =>
                    {
                        options.UseNpgsql(context.Configuration["Postgres:Connection"]);
                    });
            }).RunConsoleAsync();
    }
}