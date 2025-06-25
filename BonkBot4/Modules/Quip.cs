using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonkBot4.Modules
{
    internal class Quip : ModuleBase<SocketCommandContext>
    {
        //No attributes being used - currently 
        //primary method MakeQuipAsync()
        //requires being passed
        //message context (ISocketMessageChannel) from calling module 
        //to reply in calling message received text channel

        private IDictionary<int, string> QuipDict { get; set; }
        
        public Quip(string[] quipArr) 
        {
            if (quipArr != null)
            {
                try
                {
                    QuipDict = new Dictionary<int, string>();
                    for (int i = 0; i < quipArr.Length; i++)
                    {
                        QuipDict.Add(i, quipArr[i]);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            else { throw new Exception($"Quip not found"); }

        }

        public Quip()
        {
            //static quips for now/testing
            string[] quipArr = new string[]
            {
            "Sent that monkey to the zoo",
            "Bonk",
            "Gulag'd™",
            "Get in the hole",
            "Yeah, go ahead and clock out",
            "Dork"
            };
            QuipDict = new Dictionary<int, string>();
            try
            {
                //Add Quips to the Dictionary @ instance from quippArr/Static quips in this case
                for (int i = 0; i < quipArr.Length;i++)
                {
                    QuipDict.Add(i, quipArr[i]);

                }
            }
            catch (Exception ex)
            {
                //can be thrown to calling message channel
                throw new Exception($"Quip Static Ctor" +$"\n{ ex.Message }");
            }


        }

        //is called by another method, is passed part of the context
        //through the calling method
        public async Task MakeQuipAsync(ISocketMessageChannel contextChannel)
        {
            if (this.QuipDict != null)
            {
                //Instantiate a random, use it to generate 2 randoms, pick 1 at random to select
                //quip from QuipDict
                var rnd = new Random();
                //remove -1, read .Next
                int quip0Key = rnd.Next(0, this.QuipDict.Count - 1);
                int quip1Key = rnd.Next(0, this.QuipDict.Count - 1);
                int decider = rnd.Next(0, 1);

                if (decider == 0)
                {
                    var quip0 = QuipDict[quip0Key];
                    await contextChannel.SendMessageAsync($"{quip0}");
                    //await contextChannel.SendMessageAsync("Test Reached quip0");
                }
                else if(decider == 1)
                {
                    var quip1 = QuipDict[quip1Key];
                    await contextChannel.SendMessageAsync($"{quip1}");
                    //await contextChannel.SendMessageAsync("Test Reached quip1");
                }

                
            }

        }
    }
}
