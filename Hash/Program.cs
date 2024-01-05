using CommandLine;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Hash {
    internal class Program { 

        public class Options {
            [Option('c', "calculate", SetName = "action", HelpText = "Calculate hash for a file.")]
            public bool Calculate { get; set; }

            [Option('v', "verify", SetName = "action", HelpText = "Verify hash for a file.")]
            public bool Verify { get; set; }

            [Option('h', "hash", HelpText = "Specify hash function.")]
            public string HashFunction { get; set; } = "sha256";

            [Value(0, MetaName = "file path", HelpText = "Path to the file.")]
            public string FilePath { get; set; }

            [Value(1, MetaName = "hash", HelpText = "Hash to verify against.")]
            public string Hash { get; set; }
        }

        static void Main(string[] args) {
            try {
                Parser.Default.ParseArguments<Options>(args).WithParsed(options => RunOptions(options));
            } catch (Exception exc) {
                Console.WriteLine(exc.Message);
            }
        }

        private static void RunOptions(Options options) {
            if(options.Calculate) {
                DisplayFileHash(options.FilePath, options.HashFunction);
            } else if(options.Verify) {
                VerifyFileHash(options.FilePath, options.Hash, options.HashFunction);
            } else {
                Console.WriteLine("Please specify either -c or -v");
            }
        }

        private static void DisplayFileHash(string filePath, string hashFunction = "sha256") {
            var hash = CalculateFileHash(filePath, hashFunction);
            Console.WriteLine($"{hashFunction}: {hash}");
        }

        private static void VerifyFileHash(string filePath, string expectedHash, string hashFunction = "sha256") {
            expectedHash = expectedHash.Trim();
            if(!IsValidHash(expectedHash, hashFunction)) {
                throw new ArgumentException("Provided hash is not in valid format!");
            }
            var hash = CalculateFileHash(filePath, hashFunction);
            Console.WriteLine($"Provided hash:   {hashFunction}: {expectedHash}");
            Console.WriteLine($"Calculated hash: {hashFunction}: {hash}");
            if(CompareHashes(expectedHash, hash)) {
                Console.WriteLine("OK, provided hashes are the same.");
            } else {
                Console.WriteLine("ERROR, provided hashes differ.");
            }
        }

        private static string CalculateFileHash(string filePath, string hashFunction = "sha256") {
            string input = File.ReadAllText(filePath);
            hashFunction = hashFunction.Trim();
            hashFunction = hashFunction.ToLower();
            return hashFunction switch {
                "sha256" => CalcSha256(input),
                "md5" => CalcMd5(input),
                _ => throw new ArgumentException("Only sha256 and md5 are supported!", nameof(hashFunction)),
            };
        }

        private static string CalcMd5(string input) {
            using(MD5 md5 = MD5.Create()) {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string CalcSha256(string input) {
            using(SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static bool IsValidHash(string hash, string hashFunction = "sha256") {
            hashFunction = hashFunction.Trim();
            hashFunction = hashFunction.ToLower();
            return hashFunction switch {
                "sha256" => IsValidSha256(hash),
                "md5" => IsValidMd5(hash),
                _ => throw new ArgumentException("Only sha256 and md5 are supported!", nameof(hashFunction)),
            };
        }

        private static bool IsValidMd5(string hash) {
            Regex regex = new Regex(@"^[a-fA-F0-9]{32}$");
            return regex.IsMatch(hash);
        }

        private static bool IsValidSha256(string hash) {
            Regex regex = new Regex(@"^[a-fA-F0-9]{64}$");
            return regex.IsMatch(hash);
        }

        private static bool CompareHashes(string hash1, string hash2) {
            return hash1.Equals(hash2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
