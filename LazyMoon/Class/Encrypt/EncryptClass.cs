using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LazyMoon.Class.Encrypt
{
    /// <summary>
    /// 암호화 Class
    /// </summary>
    public static class Encryption
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int KEYSIZE = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DERIVATION_ITERATIONS = 1000;

        private static string defaultPassword = "enc";

        public static void SetDefaultPassword(string password)
        {
            defaultPassword = password;
        }

        public static string Encrypt(string plainText)
        {
            string returnValue = plainText.Encrypt(defaultPassword);
            returnValue = returnValue.Replace("/", "ㄱ");
            returnValue = returnValue.Replace("!", "ㄴ");
            returnValue = returnValue.Replace("*", "ㄷ");
            returnValue = returnValue.Replace("'", "ㄹ");
            returnValue = returnValue.Replace("(", "ㅁ");
            returnValue = returnValue.Replace(")", "ㅂ");
            returnValue = returnValue.Replace(";", "ㅅ");
            returnValue = returnValue.Replace(":", "ㅇ");
            returnValue = returnValue.Replace("@", "ㅈ");
            returnValue = returnValue.Replace("&", "ㅊ");
            returnValue = returnValue.Replace("=", "ㅋ");
            returnValue = returnValue.Replace("+", "ㅌ");
            returnValue = returnValue.Replace("$", "ㅍ");
            returnValue = returnValue.Replace(",", "ㅎ");
            returnValue = returnValue.Replace("/", "가");
            returnValue = returnValue.Replace("?", "나");
            returnValue = returnValue.Replace("#", "다");
            returnValue = returnValue.Replace("[", "라");
            returnValue = returnValue.Replace("]", "마");

            return returnValue;
        }
        public static string Decrypt(string plainText)
        {
            plainText = plainText.Replace("ㄱ", "/");
            plainText = plainText.Replace("ㄴ", "!");
            plainText = plainText.Replace("ㄷ", "*");
            plainText = plainText.Replace("ㄹ", "'");
            plainText = plainText.Replace("ㅁ", "(");
            plainText = plainText.Replace("ㅂ", ")");
            plainText = plainText.Replace("ㅅ", ";");
            plainText = plainText.Replace("ㅇ", ":");
            plainText = plainText.Replace("ㅈ", "@");
            plainText = plainText.Replace("ㅊ", "&");
            plainText = plainText.Replace("ㅋ", "=");
            plainText = plainText.Replace("ㅌ", "+");
            plainText = plainText.Replace("ㅍ", "$");
            plainText = plainText.Replace("ㅎ", ",");
            plainText = plainText.Replace("가", "/");
            plainText = plainText.Replace("나", "?");
            plainText = plainText.Replace("다", "#");
            plainText = plainText.Replace("라", "[");
            plainText = plainText.Replace("마", "]");

            return plainText.Decrypt(defaultPassword);
        }

        public static string Encrypt(this string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.
            var saltStringBytes = Generate128BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS, HashAlgorithmName.SHA1);
            var keyBytes = password.GetBytes(KEYSIZE / 8);
            using var symmetricKey = Aes.Create();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;
            using var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
            var cipherTextBytes = saltStringBytes;
            cipherTextBytes = [.. cipherTextBytes, .. ivStringBytes];
            cipherTextBytes = [.. cipherTextBytes, .. memoryStream.ToArray()];
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(this string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEYSIZE / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEYSIZE / 8).Take(KEYSIZE / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(KEYSIZE / 8 * 2).Take(cipherTextBytesWithSaltAndIv.Length - KEYSIZE / 8 * 2).ToArray();

            using var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS, HashAlgorithmName.SHA1);
            var keyBytes = password.GetBytes(KEYSIZE / 8);
            using var symmetricKey = Aes.Create();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;
            using var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
            using var memoryStream = new MemoryStream(cipherTextBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];
            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = RandomNumberGenerator.Create())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}