using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Web;
using System.Text;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public EchoDialog() {

        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("How can I help you?");
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // todo(bopan): debug the NPE issue
            //UserInfo user = InferUserInfo(activity);
            var message = await argument;
            if (string.IsNullOrEmpty(message.Text)) {
                await context.PostAsync($"empty input");
            }
            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            } 
            // job posting flow
            else if (message.Text.ToLower().Contains("hire")
                     || message.Text.ToLower().Contains("post")
                     || message.Text.ToLower().Contains("role")
                     || message.Text.ToLower().Contains("position")
                     || message.Text.ToLower().Contains("job")) {
                String rawIntention = message.Text;
                // call MS keyphase TA api, to parse out the keywords
                PostJobBody postJobBody = await CallMicroSoftAPI(context, rawIntention);

                // call LI api to post the job.
                string url = await CallPostJobAPI(context, postJobBody);
                await context.PostAsync("the url is " + url);
            }
            // save/hidde applicants flow
            //else if (message.Text.ToLower().Contains("applicants"))
            //{
            //    await context.PostAsync("No applicants available currently.");
            //}
            // tell a joke
            else if (message.Text.ToLower().Contains("a joke")) {
                await context.PostAsync("I'm a serious guy, I don't tell jokes.");
            } 
            // developer names
            else if ((message.Text.ToLower().Contains("developer") ||
                        message.Text.ToLower().Contains("developers")) &&
                       (message.Text.ToLower().Contains("name") ||
                        message.Text.ToLower().Contains("names")))
            {
                await context.PostAsync("I'm created by Bo, Srividya, Yu, and Yilong from LinkedIn jobs team.");
            }
            else
            {
                await context.PostAsync($" You just haha said {message.Text}");
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
                throw new Exception("Something is wrong with calling LinkedIn API");
            }
        }

        private async Task<String> CallPostJobAPI(IDialogContext context, PostJobBody postJobBody)
        {
            // showed interest in hiring, try to understand the user input by making calls to external API
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // convert the entities into a string
                    String entities = JsonConvert.SerializeObject(postJobBody);
                    await context.PostAsync($"Converted input for PostJob API is: {entities}");
                    httpClient.DefaultRequestHeaders.Add("X-LI-VIEWER", "HACKDAY");

                    string queryString = postJobBody.toQueryString();
                    // Request headers
                    var uri = "https://www.linkedin.com/mjobs/api/jobAssistantHackathon/jobPosting?" + queryString;
                    HttpResponseMessage httpResponse = await httpClient.GetAsync(uri);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        String jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                        await context.PostAsync($"Response received is: {jsonResponse}");
                        // convert the jobId into a view job url
                        String viewJobUrl = "https://www.linkedin.com/job/view" + jsonResponse;
                        return viewJobUrl;
                    }
                    else
                    {
                        await context.PostAsync($"Response statis received is: {httpResponse.StatusCode} {httpResponse}");
                        throw new Exception("Something is wrong with calling LinkedIn API");
                    }
                }
            }
            catch (Exception e)
            {
                await context.PostAsync($"Something is wrong with processing the request {e.ToString()}");
                throw new Exception("Something is wrong with calling LinkedIn API");
            }
        }

        private async Task<PostJobBody> CallMicroSoftAPI(IDialogContext context, String rawInput)
        {
            // showed interest in hiring, try to understand the user input by making calls to external API
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // todo(bopan): do some preprocessing for the rawInput to parse them into a few strings.
                    // for now it's just takes the entire string
                    List<String> preprocessedInput = new List<String>{rawInput};
                    DocumentInputList documentInputs = new DocumentInputList(preprocessedInput);
                    String entities = JsonConvert.SerializeObject(documentInputs);
                    await context.PostAsync($"Preprocessed input is {preprocessedInput}, Converted input for MS API is: {entities}");
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "bee7e2d91eb342dbae5860adf08082ec");

                    var queryString = HttpUtility.ParseQueryString(string.Empty);
                    // Request headers
                    var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases?" + queryString;
                    HttpResponseMessage httpResponse;
                    byte[] byteData = Encoding.UTF8.GetBytes(entities);
                    using (var content = new ByteArrayContent(byteData))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        httpResponse = await httpClient.PostAsync(uri, content);
                    }
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        String jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                        TextAnalysisKeyPhrases keyPhrases = JsonConvert.DeserializeObject<TextAnalysisKeyPhrases>(jsonResponse);

                        await context.PostAsync($"Response received is: {jsonResponse}");
                        await context.PostAsync($"The generated keywards is: {JoinStrings(keyPhrases.getKeyWordsInList())}");

                        PostJobBody postJobBody = new PostJobBody();
                        postJobBody.keywords = keyPhrases.getKeyWordsInList();
                        return postJobBody;
                    }
                    else
                    {
                        await context.PostAsync($"Response statis received is: {httpResponse.StatusCode} {httpResponse}");
                        throw new Exception("Something is wrong with calling LinkedIn API");
                    }
                }
            }
            catch (Exception e)
            {
                await context.PostAsync($"Something is wrong with processing the request {e.ToString()}");
                throw new Exception("Something is wrong with calling MicroSoft Text Analytics API");
            }
        }

        private async Task<ApplicantBody> CallGetApplicants(IDialogContext context)
        {
            // showed interest in hiring, try to understand the user input by making calls to external API
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    String target = "https://www.linkedin.com/mjobs/api/jobAssistantHackathon/jobPosting";
                    HttpResponseMessage httpResponse = await httpClient.GetAsync(target);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        String jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                        // todo convert getjsonResponse to proper list of applicants
                        await context.PostAsync($"Response received is: {jsonResponse}");
                        ApplicantBody applicantBody = JsonConvert.DeserializeObject<ApplicantBody>(jsonResponse);
                        return applicantBody;
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
                throw new Exception("Something is wrong with calling LinkedIn API");
            }
        }

        private string JoinStrings(List<string> strings) {
            if (strings == null) {
                return "null";
            }
            return String.Join(",", strings);
        }

    }
}