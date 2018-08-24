using System.Collections.Generic;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    internal class TextAnalysisKeyPhrases
    {
        public List<KeyPhrase> documents { get; set; }

        public List<string> getKeyWordsInList() {
            List<string> keywordsList = new List<string>();
            for (int i = 0; i < this.documents.Count; i++) {
                keywordsList.AddRange(documents[i].getKeyPhrases());
            }
            return keywordsList;
        }
    }

    internal class KeyPhrase {
        public string id;
        public List<string> keyPhrases;

        public KeyPhrase (string id, List<string> keyPhrases) {
            this.id = id;
            this.keyPhrases = keyPhrases;
        }

        public string getId() {
            return this.id;
        }

        public List<string> getKeyPhrases() {
            return this.keyPhrases;
        }
    }
}