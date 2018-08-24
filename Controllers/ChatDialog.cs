using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    internal class ChatDialog : IDialog<object>
    {
        string initialInput;
        public ChatDialog(string initialInput) {
            this.initialInput = initialInput;
        }
        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync($"Let's chat more!");
            await this.MessageReceivedAsync(context, initialInput);
        }

        public async Task MessageReceivedAsync(IDialogContext context, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                await context.PostAsync($"empty input");
            }
            if (message.ToLower().Contains("a joke"))
            {
                await context.PostAsync("I'm a serious guy, I don't tell jokes.");
            }
            // developer names
            else if ((message.ToLower().Contains("developer") ||
                        message.ToLower().Contains("developers")) &&
                       (message.ToLower().Contains("name") ||
                        message.ToLower().Contains("names")))
            {
                await context.PostAsync("I'm created by Bo, Srividya, Yu, and Yilong from LinkedIn jobs team.");
            } 
            // greetings
            else if (message.ToLower().Contains("hi") ||
                      message.ToLower().Contains("hello")) {
                await context.PostAsync("Hi, what can I do for you today?");
            }
            else {
                await context.PostAsync("I don't understand. Sorry!");
            }
            context.Done("");
        }
    }
}