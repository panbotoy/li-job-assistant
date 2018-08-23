using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public EchoDialog() {
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.SayAsync("How can I help you?");
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            //UserInfo user = InferUserInfo(activity);
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            } else if (message.Text.ToLower().Contains("hire")
                       || message.Text.ToLower().Contains("post")
                       || message.Text.ToLower().Contains("job")) {
                String response = await CallMicroSoftAPI(context);
            } else if (message.Text.ToLower().Contains("applicants")) {
                // user wants to see applicants of the job
            }
            else
            {
                await context.PostAsync($" You just just haha said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

        private async Task<String> CallLinkedInAPI(IDialogContext context) {
            // showed interest in hiring, try to understand the user input by making calls to external API
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    String target = "https://www.linkedin.com/mjobs/api/jobPosting/getPrefillJobId?companyName=LinkedIn&title=Software%20Engineer";
                    HttpResponseMessage httpResponse = await httpClient.GetAsync(target);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        String jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                        await context.PostAsync($"Response received is: {jsonResponse}");
                        return jsonResponse;
                    }
                    else
                    {
                        await context.PostAsync($"Response statis received is: {httpResponse.StatusCode}");
                        await context.PostAsync($"Response received is: {httpResponse.ReasonPhrase}");
                        throw new Exception("Something is wrong with calling LinkedIn API");
                    }
                }
            }
            catch (Exception e)
            {
                await context.PostAsync($"Something is wrong with processing the request {e.ToString()}");
                await context.SayAsync("Please try again");
                throw new Exception("Something is wrong with calling LinkedIn API");
            }
        }

        private async Task<String> CallMicroSoftAPI(IDialogContext context)
        {
            // showed interest in hiring, try to understand the user input by making calls to external API
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    String target = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/entities";
                    String entities = "{\"documents\":[{\"language\":\"en\",\"id\":\"1\",\"text\":\"I want to hire a software engineer for my company.\"}]}";
                    httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "bee7e2d91eb342dbae5860adf08082ec");
                    HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(target, entities);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        String jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                        await context.PostAsync($"Response received is: {jsonResponse}");
                        return jsonResponse;
                    }
                    else
                    {
                        await context.PostAsync($"Response statis received is: {httpResponse.StatusCode}");
                        await context.PostAsync($"Response received is: {httpResponse.ReasonPhrase}");
                        throw new Exception("Something is wrong with calling LinkedIn API");
                    }
                }
            }
            catch (Exception e)
            {
                await context.PostAsync($"Something is wrong with processing the request {e.ToString()}");
                await context.SayAsync("Please try again");
                throw new Exception("Something is wrong with calling MicroSoft Text Analytics API");
            }
        }

    }
}