using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Commands;
using BonkBot4.Services;
using Microsoft.Extensions.DependencyInjection;

//TODO
//Upgrade to latest discord.net version, test
//debug info from logger shows outdated socetguilduser info, e.g.  ?murderface6466?#0000
//unsure if expected or due to discord.net version being outdated with big discord api changes, though
//bot still works as expected
namespace BonkBot4
{
    internal class Program
    {
        static async Task Main()
        {
            //HostBuilder assigns KV pairs for access to our config file [appsettings.json]
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.dev.json", optional: false, reloadOnChange: true)
                        .Build();

                    x.AddConfiguration(config);
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug);
                })
                //Original template included a type declaration, <DiscordSocketClient> however it has been removed below
                //My theory is that it is expected to return that type, per the desc of the mthd, and is implicit in this [newer than template used] version of packs

                //Necessary to configure the bot to do things like it's default logging, and perhaps most importantly,
                //Message cache size for working with messages/caching messages from channels
                .ConfigureDiscordHost((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = Discord.LogSeverity.Debug,
                        AlwaysDownloadUsers = false, //Consider changing to true after base build done; may need users for getting permissions function
                        MessageCacheSize = 200,
                    };
                    config.Token = context.Configuration["Token"];
                })
                .UseCommandService((context, config) =>
                {
                    config.CaseSensitiveCommands = false;
                    config.LogLevel = Discord.LogSeverity.Debug;
                    config.DefaultRunMode = RunMode.Sync;
                })
                //Our DI Container
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<CommandHandler>();
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {  
                await host.RunAsync();
            }


        }
    }
}
