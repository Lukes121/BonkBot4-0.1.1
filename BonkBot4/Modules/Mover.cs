using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonkBot4.Modules
{
    public class Mover : ModuleBase<SocketCommandContext>
    {
        //TODO 
        //Make list of phrases to say when moving people
        //One Recomendation: "Sent that monkey to the zoo"

        //3/14 addition of string list to reply with
        /*string[] quipArr =
            [
            "Sent that monkey to the zoo",
            "Bonk",
            "Gulag'd™",
            "Get in the hole",
            "Yeah, go ahead and clock out",
            "Dork"
                ];
        Dictionary<int, string> quipDictionary = new Dictionary<int, string>();*/


        [Command("move")]
        [Alias("m", "bonk")]
        //[RequireOwner]
        public async Task MoveAsync(SocketGuildUser socketGuildUser = null, SocketGuildChannel socketGuildChannel = null)
        {
            //Quip Module Call added 3/21/24
            //Might need to nest Ifs for easier control of 'plugin' quip


            //TODO clean up/optimize logic
            //create object here to track changes to socketGuildUser instead of changing/assigning message context? Will lose efficiency


            //evaluate is guild user was mentioned, or, OR, caller as the user to move
            if (socketGuildUser == null)
            {
                bool hasUserMentions = Context.Message.MentionedUsers.Count != 0;
                if (hasUserMentions)
                {
                    //create list from context support in discord.net api, 1st user in list will be the user to act on if users are mentioned
                    //Extend this to handle multiple user mentions - will require actual error handling
                    var mentionedUsers = Context.Message.MentionedUsers as List<IGuildUser>;
                    socketGuildUser = mentionedUsers[0] as SocketGuildUser;
                }
                //You will bonk yourself if nobody is mentioned. This will not change
                else if (!hasUserMentions)
                {
                    socketGuildUser = Context.User as SocketGuildUser;
                }
                 
            }
            //evaluate if guild has AFK channel, if a channel is not mentioned (currently only working by dev mode channel ID)
            //then move above assigned socketGuildUser target to afk channel
            if (socketGuildChannel == null)
            {
                bool hasAFK = Context.Guild.AFKChannel != null;
                var mentionedChannels = Context.Message.MentionedChannels as List<IGuildChannel> ?? null;
                if (mentionedChannels == null && !hasAFK)
                    return;//add Log Message
                else if (mentionedChannels != null)
                    socketGuildChannel = mentionedChannels[0] as SocketVoiceChannel; 
                else if (hasAFK)
                    socketGuildChannel = Context.Guild.AFKChannel;
                
            }

            if (socketGuildChannel != null && socketGuildUser.VoiceState != null)
            {
                //!!!
                ///Changes to user in taskMove
                ///Message to server in taskQuip
                var contextChannel = Context.Channel;
                Quip quip = new Quip();
                var taskQuip = quip.MakeQuipAsync(contextChannel);
                var taskMove = socketGuildUser.ModifyAsync(x =>
                x.Channel = socketGuildChannel as SocketVoiceChannel);

                await Task.WhenAll(taskQuip, taskMove);
            }
            else
                return;

        }
    }
}
