using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Discord;
using System.Reflection;

namespace BonkBot4.Services
{
    public class CommandHandler : DiscordClientService
    {

        private readonly IServiceProvider _provider;
        private readonly CommandService _service;
        private readonly IConfiguration _config;

        //base there at the end is for signifying that the CTOR here is inheriting from DiscordClientService's CTOR 
        public CommandHandler(DiscordSocketClient client, ILogger<CommandHandler> logger, IServiceProvider provider, CommandService service, IConfiguration config) : base(client, logger)
        {
            this._provider = provider;
            this._service = service;
            this._config = config;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.Client.MessageReceived += HandleMessageAsync;
            _service.CommandExecuted += CommandExecutedAsync; 

            //This adds modules to the ServiceCollection: It automatically searches for attributes and will add them to the DI container
            await this._service.AddModulesAsync(Assembly.GetEntryAssembly(), this._provider);
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (result.IsSuccess)
                return;
            //Above and Below are simple boilerplate/template error reporting reccommended by template
            await commandContext.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task HandleMessageAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage message))
                return;
            if (msg.Source != MessageSource.User) return;

            //prefix of our bot command
            var argPos = 0;
            if (!message.HasStringPrefix(this._config["Prefix"], ref argPos) && !message.HasMentionPrefix(this.Client.CurrentUser, ref argPos))
                return;

            var context = new SocketCommandContext(this.Client, message);
            await this._service.ExecuteAsync(context, argPos, this._provider);
        }
    }
}
