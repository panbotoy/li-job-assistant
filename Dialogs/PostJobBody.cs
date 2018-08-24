using System.Collections.Generic;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    internal class PostJobBody
    {
        public List<string> keywords { get; set; }
        public string location { get; set; }
        public string person { get; set; }
        public string company { get; set; }

        public string toQueryString() {
            List<string> queryString = new List<string>{};
            if (this.keywords != null || this.keywords.Count > 0) {
                for (int i = 0; i < this.keywords.Count; i++ ) {
                    queryString.Add("keywords=" + this.keywords[i]);
                }
            }
            if (location != null) {
                queryString.Add("location=" + location);
            }
            if (person != null) {
                queryString.Add("person=" + person);
            }
            if (company != null)
            {
                queryString.Add("company=" + company);
            }
            return string.Join("&", queryString);
        }
    }
}