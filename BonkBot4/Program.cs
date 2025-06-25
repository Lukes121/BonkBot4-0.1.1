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
using Serilog;
using Serilog.Events;

//TODO
//
//
namespace BonkBot4
{
    internal class Program
    {
        static async Task Main()
        {
            //New methods, and revised call stack for discord.net; microsoft.extensions.hosting; etc upgrades

            //Serilog configuration; requires Serilog, Serilog.Sinks.Console
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() //consider changing this to 'information()'
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .WriteTo.Console()
                .CreateLogger();

            //var config = new HostApplicationBuilderSettings()
            var builder = Host.CreateApplicationBuilder();
            //Here is where we change the json file being used
            builder.Configuration.AddJsonFile("appsettings.dev.json", optional: false, reloadOnChange: true);
            
            //add logging
            //requires serilog.extensions.hosting
            builder.Services.AddSerilog();

            builder.Services.AddDiscordHost((config, _) =>
            {
                config.SocketConfig = new DiscordSocketConfig
                {
                    LogLevel = Discord.LogSeverity.Verbose,
                    AlwaysDownloadUsers = false,
                    MessageCacheSize = 200,
                    GatewayIntents = Discord.GatewayIntents.All //This is a new member of the discord API. Double check required intents configured in bot on discord
                };

                config.Token = builder.Configuration["Token"]!;

                //Use this to configure a custom format for Client/CommandService logging if needed. The default is below and should be suitable for Serilog usage
                config.LogFormat = (message, exception) => $"{message.Source}: {message.Message}";
            });

            builder.Services.AddCommandService((config, _) =>
            {
                config.DefaultRunMode = RunMode.Async;
                config.CaseSensitiveCommands = false;
                
            });

            builder.Services.AddInteractionService((config, _) =>
            {
                config.LogLevel = Discord.LogSeverity.Verbose; //consider changing to .info
                config.UseCompiledLambda = true; //?
            });

            //Add each service to the builder
            //May need to change to InteractionService if <CommandService> is deprecated. 
            //Review what Interaction Service can do that CommandService cannot
            builder.Services.AddHostedService<CommandHandler>();

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            //Below is the old program flow with discord.net v2, and hostbuilder.
            //The Hostbuilder has been partially replaced with new hosting methods, and the below code is left for reference.

            //HostBuilder assigns KV pairs for access to our config file [appsettings.json]

            /*

            var builder1 = new HostBuilder()
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
                
                // /*
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
            */
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            var host = builder.Build();

            await host.RunAsync();

        }
    }
}
