namespace CodingComparison {
    internal class ExampleGenerator {
        private const string letters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM .,\"\'";
        private const string lettersPL = "ąćęłńóśżźĄĆĘŁŃÓŚŻŹ";
        private const int K = 1000;
        private const int M = 1000000;
        private const int B = 1000000000;
        private const int TINY = 10 * K;
        private const int SMALL = 100 * K;
        private const int MEDIUM = M;
        private const int BIG = 100 * M;
        private const int LARGE = B;
        private string Directory { get; }
        private Random RNG { get; } = new Random();
        public ExampleGenerator(string path) {
            Directory = path;
        }

        public void GenAll() {
            //Random
            GenRandom(false, TINY);
            GenRandom(true, TINY);
            GenRandom(false, SMALL);
            GenRandom(true, SMALL);
            GenRandom(false, MEDIUM);
            GenRandom(true, MEDIUM);
            GenRandom(false, BIG);
            GenRandom(true, BIG);
            GenRandom(false, LARGE);
            GenRandom(true, LARGE);
            //Books
            GenBooks(false); //EN
            GenBooks(true); //PL
        }

        public void GenRandom(bool pl = false, int size = MEDIUM) {
            string elements = letters;
            string text = string.Empty;
            string fname = "random";
            if(pl) {
                elements += lettersPL;
                fname += "PL";
            }
            for(int i = 0; i < size; i++) {
                text += elements.ElementAt(RNG.Next(0, elements.Length - 1));
            }
            fname += "_" + GetSizeDescription(size) + ".txt";
            File.WriteAllText(Path.Combine(Directory, fname), text);
        }

        public void GenBooks(bool pl = false) {

        }

        private static string GetSizeDescription(int size) {
            if(size < 1000) {
                return size.ToString();
            }
            int exp = (int)(Math.Log(size) / Math.Log(1000));
            char prefix = "kMGT"[exp - 1];
            return $"{size / Math.Pow(1000, exp):0.##}{prefix}";
        }
    }
}
