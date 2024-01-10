using CommandLine;
using System.Security.Cryptography;
using System.Text;

namespace Crypt {
    public class Program {

        public class Options {
            [Option('e', "encrypt", HelpText = "Encrypt the file")]
            public bool Encrypt { get; set; }

            [Option('d', "decrypt", HelpText = "Decrypt the file")]
            public bool Decrypt { get; set; }

            [Value(0, MetaName = "file_path", HelpText = "The path to the file to encrypt or decrypt")]
            public string FilePath { get; set; }

            [Option('s', "symmetric", HelpText = "Use symmetric encryption")]
            public bool Symmetric { get; set; }

            [Option('a', "asymmetric", HelpText = "Use asymmetric encryption")]
            public bool Asymmetric { get; set; }

            [Option("key", HelpText = "The key for symmetric encryption")]
            public string Key { get; set; }

            [Option("public_key", HelpText = "The path to the public key for asymmetric encryption")]
            public string PublicKeyPath { get; set; }

            [Option("private_key", HelpText = "The path to the private key for asymmetric encryption")]
            public string PrivateKeyPath { get; set; }
        }

        static void Main(string[] args) {
            try {
                Parser.Default.ParseArguments<Options>(args).WithParsed(options => RunOptions(options));
            } catch (Exception exc) {
                Console.WriteLine(exc.Message);
            }
        }

        private static void RunOptions(Options options) {
            if (options.Encrypt && options.Symmetric) {
                EncryptWithSymmetricKey(options.FilePath, options.Key);
            } else if (options.Encrypt && options.Asymmetric) {
                EncryptWithAsymmetricKey(options.FilePath, options.PublicKeyPath);
            } else if (options.Decrypt && options.Symmetric) {
                DecryptWithSymmetricKey(options.FilePath, options.Key);
            } else if (options.Decrypt && options.Asymmetric) {
                DecryptWithAsymmetricKey(options.FilePath, options.PrivateKeyPath);
            } else {
                Console.WriteLine("Invalid options provided.");
            }
        }

        #region AES
        public static void EncryptWithSymmetricKey(string filePath, string password) {
            using (AesManaged aesAlg = new AesManaged()) {
                byte[] key = GenerateKey(password, aesAlg.KeySize);
                byte[] iv = aesAlg.IV;
                using (FileStream fsOutput = new FileStream(filePath + ".crypt", FileMode.Create)) {
                    fsOutput.Write(iv, 0, iv.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(fsOutput, aesAlg.CreateEncryptor(key, iv), CryptoStreamMode.Write)) {
                        using (FileStream fsInput = new FileStream(filePath, FileMode.Open)) {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0) {
                                csEncrypt.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }

        public static void DecryptWithSymmetricKey(string filePath, string password) {
            using (AesManaged aesAlg = new AesManaged()) {
                byte[] key = GenerateKey(password, aesAlg.KeySize);
                using (FileStream fsInput = new FileStream(filePath, FileMode.Open)) {
                    byte[] iv = new byte[aesAlg.IV.Length];
                    fsInput.Read(iv, 0, iv.Length);
                    using (CryptoStream csDecrypt = new CryptoStream(fsInput, aesAlg.CreateDecryptor(key, iv), CryptoStreamMode.Read)) {
                        using (FileStream fsOutput = new FileStream(filePath.Replace(".crypt", ""), FileMode.Create)) {
                            byte[] buffer = new byte[4096];
                            int bytesRead;

                            while ((bytesRead = csDecrypt.Read(buffer, 0, buffer.Length)) > 0) {
                                fsOutput.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] GenerateKey(string password, int keySize) {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("S@lt"), 10000)) {
                return deriveBytes.GetBytes(keySize / 8);
            }
        }
        #endregion

        #region RSA
        public static void EncryptWithAsymmetricKey(string filePath, string publicKeyPath) {
            string plainText = File.ReadAllText(filePath);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var rsa = new RSACryptoServiceProvider()) {
                string publicKeyPEM = File.ReadAllText(publicKeyPath);
                rsa.ImportFromPem(publicKeyPEM);

                byte[] encryptedBytes = rsa.Encrypt(plainTextBytes, RSAEncryptionPadding.Pkcs1);

                File.WriteAllBytes(filePath + ".crypt", encryptedBytes);
            }
        }


        public static void DecryptWithAsymmetricKey(string filePath, string privateKeyPath) {
            byte[] cipherText = File.ReadAllBytes(filePath);

            using (var rsa = new RSACryptoServiceProvider()) {
                string privateKeyPEM = File.ReadAllText(privateKeyPath);
                rsa.ImportFromPem(privateKeyPEM);

                byte[] decryptedBytes = rsa.Decrypt(cipherText, false);
                string decrypted = Encoding.UTF8.GetString(decryptedBytes);

                File.WriteAllText(filePath.Replace(".crypt", ""), decrypted);
            }
        }

        #endregion
    }
}
