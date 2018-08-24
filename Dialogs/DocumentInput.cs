using System.Collections.Generic;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    // takes a list of rawInput from user. 
    // this class will be converted to the call to MS endpoint
    internal class DocumentInputList
    {
        public IList<DocumentInput> documents { get; set; }
        public DocumentInputList(List<string> rawInput)
        {
            this.documents = new List<DocumentInput>();
            for (int i = 0; i < rawInput.Count; i++) {
                string index = (i + 1).ToString();
                DocumentInput document = new DocumentInput("en", index, rawInput[i]);
                this.documents.Add(document);
            }
        }
    }

    internal class DocumentInput
    {
        public string language { get; set; }
        public string id { get; set; }
        public string text { get; set; }

        public DocumentInput(string language, string id, string text)
        {
            this.language = language;
            this.id = id;
            this.text = text;
        }
    }
}