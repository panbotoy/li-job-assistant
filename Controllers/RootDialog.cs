using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        async Task IDialog<object>.StartAsync(IDialogContext context)
        {
            // Root dialog initiates and waits for the next message from the user. 
            // When a message arrives, call MessageReceivedAsync.
            //await context.PostAsync("How can I help you?");
            context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try {
                var message = await result; // We've got a message!
                if (string.IsNullOrEmpty(message.Text))
                {
                    await context.PostAsync($"empty input");
                    context.Wait(MessageReceivedAsync);
                }
                if (message.Text.Contains("applicants")
                   || message.Text.Contains("applicant")
                    || message.Text.Contains("applications")
                   )
                {
                    context.Call(new ApplicantsDialog(), this.ResumeAfterNewOrderDialog);
                }
                else if (message.Text.ToLower().Contains("hire")
                         || message.Text.ToLower().Contains("post")
                         || message.Text.ToLower().Contains("role")
                         || message.Text.ToLower().Contains("position")
                         || message.Text.ToLower().Contains("job"))
                {

                    context.Call(new EchoDialog(message.Text), this.ResumeAfterNewOrderDialog);
                }
                else
                {
                    context.Call(new ChatDialog(message.Text), this.ResumeAfterNewOrderDialog);
                }
            } catch (Exception e) {
                await context.PostAsync($"Something is to ininitialize the request {e.ToString()}");
                //throw new Exception("Something is wrong with calling LinkedIn API");
            }

        }

        private async Task ResumeAfterNewOrderDialog(IDialogContext context, IAwaitable<object> result)
        {
            // Store the value that NewOrderDialog returned. 
            // (At this point, new order dialog has finished and returned some value to use within the root dialog.)
            var resultFromNewOrder = await result;

            //await context.PostAsync($"{resultFromNewOrder}");

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);
        }
    }
}