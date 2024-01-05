using CommandLine;
using System.Security.Cryptography;
using System.Text;

namespace Crypt {
    internal class Program {

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
            } catch(Exception exc) {
                Console.WriteLine(exc.Message);
            }
        }

        private static void RunOptions(Options options) {
            if(options.Encrypt && options.Symmetric) {
                EncryptWithSymmetricKey(options.FilePath, options.Key);
            } else if(options.Encrypt && options.Asymmetric) {
                EncryptWithAsymmetricKey(options.FilePath, options.PublicKeyPath);
            } else if(options.Decrypt && options.Symmetric) {
                DecryptWithSymmetricKey(options.FilePath, options.Key);
            } else if(options.Decrypt && options.Asymmetric) {
                DecryptWithAsymmetricKey(options.FilePath, options.PrivateKeyPath);
            } else {
                Console.WriteLine("Invalid options provided.");
            }
        }

        #region AES
        static void EncryptWithSymmetricKey(string filePath, string password) {
            string plainText = File.ReadAllText(filePath);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            using(AesManaged aes = new AesManaged()) {
                aes.GenerateIV();
                aes.GenerateKey();
                using(RijndaelManaged rijndael = new RijndaelManaged()) {
                    rijndael.Mode = CipherMode.ECB;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.BlockSize = 128;
                    using(ICryptoTransform encryptor = rijndael.CreateEncryptor(aes.Key, aes.IV)) {
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                        File.WriteAllBytes(filePath + ".crypt", encryptedBytes);
                    }
                }
            }
        }

        static void DecryptWithSymmetricKey(string filePath, string password) {
            byte[] cipherText = File.ReadAllBytes(filePath);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            using(AesManaged aes = new AesManaged()) {
                aes.GenerateIV();
                aes.GenerateKey();
                using(RijndaelManaged rijndael = new RijndaelManaged()) {
                    rijndael.Mode = CipherMode.ECB;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.BlockSize = 128;
                    using(ICryptoTransform decryptor = rijndael.CreateDecryptor(aes.Key, aes.IV)) {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                        string decrypted = Encoding.UTF8.GetString(decryptedBytes);
                        File.WriteAllText(filePath.Replace(".crypt", ""), decrypted);
                    }
                }
            }
        }
        #endregion

        #region RSA
        private static void EncryptWithAsymmetricKey(string filePath, string publicKeyPath) {
            string plainText = File.ReadAllText(filePath);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using(var rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(File.ReadAllText(publicKeyPath));
                byte[] encryptedBytes = rsa.Encrypt(plainTextBytes, false);
                File.WriteAllBytes(filePath + ".crypt", encryptedBytes);
            }
        }

        private static void DecryptWithAsymmetricKey(string filePath, string privateKeyPath) {
            byte[] cipherText = File.ReadAllBytes(filePath);
            using(var rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(File.ReadAllText(privateKeyPath));
                byte[] decryptedBytes = rsa.Decrypt(cipherText, false);
                string decrypted = Encoding.UTF8.GetString(decryptedBytes);
                File.WriteAllText(filePath.Replace(".crypt", ""), decrypted);
            }
        }
        #endregion
    }
}
