using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    internal class ApplicantBody
    {
        public List<Applicant> applicants = new List<Applicant>{};
    }

    [Serializable]
    class Applicant{
        public string memberId { get; set; }
        public string memberName { get; set; }
        public string memberEmail { get; set; }
        public string memberCompany { get; set; }
        public string memberTitle { get; set; }
        public string memberImageUrl { get; set; }
        public string jobId { get; set; }
        public string jobTitle { get; set; }
        public bool saved { get; set; }
    }
}