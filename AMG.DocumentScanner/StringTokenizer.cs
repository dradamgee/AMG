using System.Collections.Generic;
using System.Linq;

namespace AMG.DocumentScanner {
    public class StringTokenizer {
        public IEnumerable<string> Tokenize(IEnumerable<string> lines) {
            return lines.SelectMany(line => line.Split(Separators));
        }

        private char[] Separators  = " ,.:;-?!''()=".ToArray();        
    }
}