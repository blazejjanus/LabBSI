using Crypt;

namespace Tests {
    [TestClass]
    public class CryptTests {
        private string _filePath = "test.txt";
        public CryptTests() {
            if(!File.Exists(_filePath)) {
                File.WriteAllText(_filePath, "Ala ma kota.");
            }
        }

        [TestMethod]
        public void TestEncrypt() {
            Crypt.Program.EncryptWithSymmetricKey(_filePath, "password");
        }

        [TestMethod]
        public void TestDecrypt() {
            Crypt.Program.DecryptWithSymmetricKey(_filePath+".crypt", "password");
        }
    }
}