using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel.Primitives;


namespace BonkBot4.Modules
{
    internal class AIQuip : ModuleBase<SocketCommandContext>
    {
        //No attributes being used - currently 
        //primary method MakeQuipAsync()
        //requires being passed
        //message context (ISocketMessageChannel) from calling module 
        //to reply in calling message received text channel

        //TODO
        //Implement API call to openAI to make dynamic quips

        //Should this be just for generating an AI Quip? Or should this interact with the AI, and Quip responsible for the prompt and etc to this?

        //private IDictionary<int, string> QuipDict { get; set; }

        public string QuipString { get; set; }

        /*public AIQuip(string quipString)
        {
            OpenAIClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        }*/

        //Use Responses API to get a response
        //requires openAI; .Responses isn't available in this package, but is on the github doc???
        public void GetOpenAIAPIResponse()
        {
            //OpenAIClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            //Include token limit
            //
            ChatClient client = new(
                model: "gpt-4.1-mini",
                apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                );

            //Prompts
            //generate a quip for me. A quip shouldn't exceed 200 characters, should be a single sentence, and should be shorter. The quip should be mildly insulting and or inappropriate. Speak like a zoomer, and use gaming pop culture references like 'spirit bombed' or for example 'sent that monkey to the zoo' or 'gulag'd™'. Now generate a quip"
            
            
            ChatCompletion completion = client.CompleteChat("Generate a quip. Quips should be a few words, mildly insulting or inappropriate, and should generally insult personality or intelligence. Otherwise, single words like 'deleted' or 'gulag'd™ will do. Now generate a quip");
            QuipString = ($"{completion.Content[0].Text}");
        }

        //in method, send message async like this
        //await contextChannel.SendMessageAsync($"{quip1}");
        public async Task MakeAIQuipAsync(ISocketMessageChannel contextChannel)
        {
            await contextChannel.SendMessageAsync((QuipString)??"Quipstring is null");
        }
    }
}
