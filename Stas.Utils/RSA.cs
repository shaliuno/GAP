using System.Security.Cryptography;
using System.Text;

namespace Stas.Utils;

public class RSA {
    public static void TestCripto() {
        UnicodeEncoding ByteConverter = new UnicodeEncoding();

        byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
        byte[] encryptedData;
        byte[] decryptedData;

        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider()) {
            var encr_param = RSA.ExportParameters(false);
            encryptedData = RSAEncrypt(dataToEncrypt, encr_param, false);
            decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

           var res = ByteConverter.GetString(decryptedData);
        }
    }

    public static byte[] RSAEncrypt(byte[] Data, RSAParameters RSAKeyInfo, bool Padding) {
        try {
            byte[] encryptedData;
            using (var RSA = new RSACryptoServiceProvider()) {
                RSA.ImportParameters(RSAKeyInfo);
                encryptedData = RSA.Encrypt(Data, Padding);
            }
            return encryptedData;
        }
        catch (CryptographicException e) {
            Console.WriteLine(e.Message);

            return null;
        }
    }

    public static byte[] RSADecrypt(byte[] Data, RSAParameters RSAKeyInfo, bool Padding) {
        try {
            byte[] decryptedData;
            using (var RSA = new RSACryptoServiceProvider()) {
                RSA.ImportParameters(RSAKeyInfo);
                decryptedData = RSA.Decrypt(Data, Padding);
            }
            return decryptedData;
        }
        catch (CryptographicException e) {
            Console.WriteLine(e.ToString());

            return null;
        }
    }
}
