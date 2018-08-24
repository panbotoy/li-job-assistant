using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    internal class ApplicantsDialog : IDialog<object>
    {
        private ApplicantBody applicants = null;
        private int applicationIndex = 0;
        public ApplicantsDialog() {
        }
        public async Task StartAsync(IDialogContext context)
        {
            // fetch # of applicants
            this.applicants = await CallGetApplicants(context);
            await context.PostAsync($"You have {this.applicants.applicants.Count} applicants. Would you like to review?");
            // todo: need to add a prompt
            context.Wait(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result; // We've got a message!
            if ((message.Text.ToLower().Contains("yes")
                 || message.Text.ToLower().Contains("sure")
                 || message.Text.ToLower().Contains("okay")
                 || message.Text.ToLower().Contains("review")) 
                && this.applicationIndex < this.applicants.applicants.Count) // if there are more applicants to view
            {
                await DisplayHeroCard(context, this.applicants.applicants[applicationIndex]);
                // User said 'order', so invoke the New Order Dialog and wait for it to finish.
                // Then, call ResumeAfterNewOrderDialog.
                // need to 
                //await context.Forward(new ApplicantsDialog(), HandleResumeAfter, message, CancellationToken.None);
                //await context.Call(new ApplicantsDialog(), ResumeAfterNewOrderDialogAsync);
                // prompt for good/bad
                // choose top of the list, and display to the user, prompt for input
                this.applicationIndex++;
                if (this.applicants.applicants.Count - this.applicationIndex > 0) {
                    await context.PostAsync($"You have {this.applicants.applicants.Count - this.applicationIndex} " +
                                        "more applicants. Would you like to review?");
                    context.Wait(this.MessageReceivedAsync);
                } else {
                    await context.PostAsync($"No more applicants to view. Come back later!");
                    context.Done("Finished");
                }
            } else {
                // if user says no, then terminate the task
                await context.PostAsync($"Leaving the applicant view dialog, bye!");
                context.Done("Finished");
            }
            // User typed something else; for simplicity, ignore this input and wait for the next message.
        }

        //private async Task ResumeAfterNewOrderDialogAsync(IDialogContext context, IAwaitable<string> result)
        //{
        //    // Store the value that NewOrderDialog returned. 
        //    // (At this point, new order dialog has finished and returned some value to use within the root dialog.)
        //    var resultFromNewOrder = await result;

        //    await context.PostAsync($"New order dialog just told me this: {resultFromNewOrder}");

        //    // Again, wait for the next message from the user.
        //    context.Done("Finished_ApplicantsLog");
        //}

        public async Task DisplayHeroCards(IDialogContext context, ApplicantBody applicantBody)
        {
            List<Attachment> attachments = new List<Attachment>();
            for (int i = 0; i < applicantBody.applicants.Count; i++) {
                Attachment attachment = GetProfileHeroCard(applicantBody.applicants[i]);
                attachments.Add(attachment);
            }
            var replyMessage = context.MakeMessage();
            replyMessage.Attachments = attachments;
            await context.PostAsync(replyMessage);
        }

        public async Task DisplayHeroCard(IDialogContext context, Applicant applicant)
        {
            List<Attachment> attachments = new List<Attachment>();
            Attachment attachment = GetProfileHeroCard(applicant);
            attachments.Add(attachment);
            var replyMessage = context.MakeMessage();
            replyMessage.Attachments = attachments;
            await context.PostAsync(replyMessage);
        }

        private static Attachment GetProfileHeroCard(Applicant applicant)
        {
            var heroCard = new HeroCard
            {
                // title of the card  
                Title = applicant.memberName,
                //subtitle of the card  
                Subtitle = applicant.memberTitle + "@" + applicant.memberCompany,
                // navigate to page , while tab on card  
                Tap = new CardAction(ActionTypes.OpenUrl, "View profile on LinkedIn", value: "http://www.devenvexe.com"),
                //Detail Text  
                Text = "applicant for job you posted: " + applicant.jobTitle,
                // list of  Large Image  
                Images = new List<CardImage> { new CardImage(applicant.memberImageUrl) },
                // list of buttons   
                //Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://www.devenvexe.com") }
                Buttons = new List<CardAction> {}
            };

            return heroCard.ToAttachment();
        }

        private async Task<ApplicantBody> CallGetApplicants(IDialogContext context)
        {
            // showed interest in hiring, try to understand the user input by making calls to external API
            //try
            //{
            //    using (HttpClient httpClient = new HttpClient())
            //    {
            //        String target = "https://www.linkedin.com/mjobs/api/jobAssistantHackathon/jobPosting";
            //        HttpResponseMessage httpResponse = await httpClient.GetAsync(target);
            //        if (httpResponse.IsSuccessStatusCode)
            //        {
            //            String jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            //            // todo convert getjsonResponse to proper list of applicants
            //            await context.PostAsync($"Response received is: {jsonResponse}");
            //            ApplicantBody applicantBody = JsonConvert.DeserializeObject<ApplicantBody>(jsonResponse);
            //            return applicantBody;
            //        }
            //        else
            //        {
            //            await context.PostAsync($"Response statis received is: {httpResponse.StatusCode}");
            //            await context.PostAsync($"Response received is: {httpResponse.ReasonPhrase}");
            //            throw new Exception("Something is wrong with calling LinkedIn API");
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    await context.PostAsync($"Something is wrong with processing the request {e.ToString()}");
            //    throw new Exception("Something is wrong with calling LinkedIn API");
            //}
            Applicant applicant1 = new Applicant();
            applicant1.memberName = "Bo Pan";
            applicant1.memberEmail = "bopan@linkedin.com";
            applicant1.memberCompany = "LinkedIn";
            applicant1.memberTitle = "Software Engineer";
            applicant1.memberImageUrl = "https://images.pexels.com/photos/460823/pexels-photo-460823.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=350";
            applicant1.jobTitle = "Software Breaker";

            Applicant applicant2 = new Applicant();
            applicant2.memberName = "Yu Wang";
            applicant2.memberEmail = "ywang8@linkedin.com";
            applicant2.memberCompany = "LinkedIn";
            applicant2.memberTitle = "Super Software Engineer";
            applicant2.memberImageUrl = "https://images.pexels.com/photos/39317/chihuahua-dog-puppy-cute-39317.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=350";
            applicant2.jobTitle = "Software Breaker";

            ApplicantBody applicantBody = new ApplicantBody();
            applicantBody.applicants = new List<Applicant> { applicant1, applicant2 };
            return applicantBody;
        }
    }
}