using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Conveyor.NetCore.Test")]

namespace Conveyor.Utility
{
    /// <summary>
    /// This class has been inspired by <a href="https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp">Stackoverflow</a>
    /// </summary>
    internal static class StringCipher
    {

        private static readonly string SecretKey = Environment.GetEnvironmentVariable("CONVEYOR_SECRET_KEY");

		// This constant determines the number of iterations for the password bytes generation function.
		private const int DerivationIterations = 2048;

		/// <summary>
		/// Encrypts given string
		/// </summary>
		/// <param name="plainText">the string to be encrypted</param>
		/// <returns>encrypted string</returns>
		public static string Encrypt(string plainText)
        {
            ValidateSecretKey();
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate128BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();

            using (var memoryStream = new MemoryStream())
            using (var password = new Rfc2898DeriveBytes(SecretKey, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(16);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        // Start by writing salt and iv
                        memoryStream.Write(saltStringBytes, 0, saltStringBytes.Length);
                        memoryStream.Write(ivStringBytes, 0, ivStringBytes.Length);
                        // Append encrypted content
                        streamWriter.Write(plainText);
                    }
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }


		/// <summary>
		/// Validates whether the secret key has been set and meets the required security standards. The secret key
		/// must be provided via an environment variable and should contain at least one lowercase letter, one uppercase letter,
		/// one digit, one special character, and be at least 12 characters long.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when the secret key has not been set, is empty, or does not meet security standards.</exception>
		private static void ValidateSecretKey()
		{
			if (string.IsNullOrEmpty(SecretKey))
			{
				throw new InvalidOperationException("Secret key is required and not set. Have you set the `CONVEYOR_SECRET_KEY` environment variable?");
			}

			var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\^\$*.\[\]{}\(\)?-“!@#%&/,><':;|_~`])\S{12,}$");
			if (!regex.IsMatch(SecretKey))
			{
				throw new InvalidOperationException("Secret key does not meet the security standards. " +
													"It must contain at least one lowercase letter, one uppercase letter, " +
													"one digit, one special character, and be at least 12 characters long.");
			}
		}


		/// <summary>
		/// Decrypts given encoded string
		/// </summary>
		/// <param name="cipheredText">the encoded string to be decrypted</param>
		/// <returns>Decoded string</returns>
		public static string Decrypt(string cipheredText)
		{
			ValidateSecretKey();
			try
            {
                return Decrypt(cipheredText, 128);
            }
            catch (Exception ex)
            {
                if (ex is PlatformNotSupportedException || ex is CryptographicException)
                    return Decrypt(cipheredText, 256);
                else
                    throw;
            }

            string Decrypt(string cipherText, int blockSize)
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(blockSize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(blockSize / 8).Take(blockSize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(blockSize / 8 * 2).ToArray();

                using (var password = new Rfc2898DeriveBytes(SecretKey, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(blockSize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = blockSize;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
            
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 16 Bytes will give us 128 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
