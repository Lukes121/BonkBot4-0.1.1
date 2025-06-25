using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonkBot4.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        //Alias is a way to add new strings to the command attribute
        [Alias("p", "pin", "test")]
        
        //A way to require any kind of permissions ||||TODO Consider a way to make texasrangers.Access permission level for this bot 
        //[RequireUserPermission(Discord.GuildPermission.Administrator)]
        
        //a way to require permission of the bot
        //[RequireBotPermission(Discord.GuildPermission.Administrator)]
        
        //requires that the command be run by the OWNER OF THE BOT i.e. me
        [RequireOwner]
        public async Task PingAsync()
        {
            await Context.Channel.TriggerTypingAsync();
            //await ReplyAsync("Pong!");
            await Context.Channel.SendMessageAsync("Pong!");
            //await Context.User.CreateDMChannelAsync("Hey! This is a private message!");
        }

        [Command("info")]
        public async Task InfoAsync(SocketGuildUser socketGuildUser = null)
        {
            if (socketGuildUser == null)
            {
                socketGuildUser = Context.User as SocketGuildUser;
            }

            await ReplyAsync($"ID: {socketGuildUser.Id}\n" +
                $"Name: {socketGuildUser.Username}#{socketGuildUser.Discriminator}\n" +
                $"Created at: {socketGuildUser.JoinedAt}");
        }


        //Troubleshooting Command to gain info on handling of vchannels in-context
        //Should delete when done, or change perm requirments to admin/mod only
        [Command("ReadVChannels")]
        [Alias("r")]
        [RequireOwner]
        public async Task ReadVChannelsAsync()
        {
            try {
                bool hasAFK = Context.Guild.AFKChannel != null;
                bool hasVoiceChannels = Context.Guild.VoiceChannels != null;

                //If there's an AFK channel, reply to the context with it
                if (hasAFK)
                {
                    var AFKChannel = Context.Guild.AFKChannel;
                    await ReplyAsync($"AFK: {AFKChannel.ToString()}");
                    //await ReplyAsync($"{voiceChannels.ToString()}");
                }
                //Read and reply with voice channels.
                //We are excluding the AFK channel
                //so it may be more efficient to remove the boolean logic and do it all in 
                //1 step with below ish logic
                if (hasVoiceChannels)
                {
                    var voiceChannels = Context.Guild.VoiceChannels;
                    List<String> vChannelsList = new List<string>();
                    foreach ( var voiceChannel in voiceChannels )
                    {
                        bool isAFK = voiceChannel == Context.Guild.AFKChannel;
                        if (!isAFK)
                        {
                            vChannelsList.Add(voiceChannel.Name);
                            //await ReplyAsync(voiceChannel.Name);
                        }
                        else if (isAFK)
                            continue;

                    }
                    string vChannelNamesString = "";
                    foreach (string channel in vChannelsList)
                    {
                        vChannelNamesString += $"{channel}\n";
                    }
                    await ReplyAsync(vChannelNamesString);
                }

                else
                    return;

                }
            catch (Exception ex)
            {
                await ReplyAsync($"ReadVChannel Failed; \n" +$"{ex}");
            }
        }


    }
}
