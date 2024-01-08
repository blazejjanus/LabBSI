using CommandLine;

namespace CodingComparison {
    internal class Program {
        public class Options {
            [Option('f', "file", SetName = "action", HelpText = "Input file path.")]
            public string? File { get; set; }

            [Option('d', "directory", SetName = "action", HelpText = "Directory containing text files.")]
            public string? Directory { get; set; }

            [Option('g', "generate", SetName = "action", HelpText = "Generate test files in this directory.")]
            public string? Generate { get; set; }
        }

        static void Main(string[] args) {
            try {
                Parser.Default.ParseArguments<Options>(args).WithParsed(options => RunOptions(options));
            } catch(Exception exc) {
                Console.WriteLine(exc.ToString());
            }
        }

        private static void RunOptions(Options options) {
            Dictionary<string, string> files;
            if(options.File != null) {
                files = ReadFile(options.File);
            }else if(options.Directory != null) {
                files = ReadDirectory(options.Directory);
            }else if(options.Generate != null){
                var generator = new ExampleGenerator(options.Generate);
                generator.GenAll(); return;
            }else {
                throw new Exception("No valid option provided.");
            }
            RunBenchmark(files);
        }

        static void RunBenchmark(Dictionary<string, string> files) {
            var results = new List<Result>();
            Console.WriteLine($"Starting Benchmark for {files.Count} files.");
            for(int i = 0; i < files.Count; i++) {
                Console.WriteLine($"Processing file {i + 1}/{files.Count}...");
                Console.WriteLine("\tCharacters present in file:");
                int line = 0;
                foreach(var c in FileStats.GetCharStats(files.ElementAt(i).Value)) {
                    if(line%4 == 0) { Console.Write("\t"); }
                    Console.Write("\t" + c.ToString());
                    if(line % 4 == 3) { Console.Write("\n"); }
                    line++;
                }
                Console.Write("\n");
                Console.Write("\tStarting ANS...    ");
                results.Add(RunANS());
                Console.Write("\t\t\t\t\t\t\t\t\tDone.\n");
                Console.Write("\tStarting Huffman...");
                results.Add(RunHuffman());
                Console.Write("\t\t\t\t\t\t\t\t\tDone.\n");
            }
        }

        static Result RunANS() {
            //throw new NotImplementedException();
            return null;
        }

        static Result RunHuffman() {
            //throw new NotImplementedException();
            return null;
        }

        #region Reading Input
        static Dictionary<string, string> ReadFile(string path) {
            string fname = Path.GetFileName(path);
            string content = File.ReadAllText(path);
            if(content.Length <= 0) throw new Exception($"Provided file {fname} is empty!");
            return new() { { fname, content } };
        }

        static Dictionary<string, string> ReadDirectory(string path) {
            var result = new Dictionary<string, string>();
            foreach(string file in Directory.GetFiles(path, "*.txt")) {
                try { //Catch exception for single file and omit it
                    var rf = ReadFile(path);
                    result.Add(rf.ElementAt(0).Key, rf.ElementAt(0).Value);
                }catch(Exception exc) {
                    Console.WriteLine($"Error reading file {file}: {exc.Message}, skipping.");
                    continue;
                }
            }
            return result;
        }
        #endregion
    }
}
