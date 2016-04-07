using System.Collections.Generic;
using System.Linq;
using AMG.Collections;

namespace AMG.DocumentScanner {
    public class DocumentScanner {
        
        public IDictionary<string, int> Scan(IEnumerable<string> words) {
            IDictionary<string, int> dictionary = new Dictionary<string, int>();
            //IDictionary<string, int> dictionary = new TreeDictionary<string, char, int>(new LetterNode());

            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(word))
                    continue;

                int value;
                if (dictionary.TryGetValue(word, out value))
                {
                    dictionary[word] = value + 1;
                }
                else
                {
                    dictionary[word] = 1;
                }
            }

            return dictionary;
        }

        public static IEnumerable<string> WriteResults(IDictionary<string, int> dictionary) {
            return dictionary.Select(keyValuePair => string.Format("{0} : {1}", keyValuePair.Key, keyValuePair.Value));
        }
    }
}