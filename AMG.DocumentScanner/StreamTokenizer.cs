using System.Collections.Generic;
using System.IO;

namespace AMG.DocumentScanner {
    public class StreamTokenizer {

        public StreamTokenizer() {
            m_delimiters = new List<char>(){' ', '!'};
        }

        IList<char> m_delimiters;

        public IEnumerable<string> Tokenize(Stream stream) {
            var word = string.Empty;

            int characterCode = stream.ReadByte();
            while (characterCode != -1) {
                var character = (char)characterCode;

                if (IsDelimiter(character)) {
                    if (word != string.Empty)
                    {
                        yield return word;
                        word = string.Empty;
                    }
                } else {
                    word += character;
                }

                characterCode = stream.ReadByte();
            }
            if (word != string.Empty) {
                yield return word;
            }
        }

        private bool IsDelimiter(char character) {
            return m_delimiters.Contains(character);
        }
    }
}


