using System.Security.Cryptography;
using System.Text;
using Domain.Models.Dayforce;

namespace Services.Helpers
{
    public class EncryptionHelper : IEncrytionHelper
	{
        private readonly OtherSecrets _secrets;
        private readonly string IVCONST = "TheGreatest@2024"; // Todo: Do not hardcode IV this is against the fundamentals

        public EncryptionHelper(OtherSecrets otherSecrets)
		{
            _secrets = otherSecrets;
		}

        public string? Decrypt(string? encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue)) return encryptedValue;

            var cipherBytes = Convert.FromBase64String(encryptedValue);

            var key = Encoding.UTF8.GetBytes(_secrets.AesKey);
            var iv = Encoding.UTF8.GetBytes(IVCONST);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Padding = PaddingMode.PKCS7;

                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    byte[] decryptedBytes;
                    using (var msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var msPlain = new MemoryStream())
                            {
                                csDecrypt.CopyTo(msPlain);
                                decryptedBytes = msPlain.ToArray();
                            }
                        }
                    }
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        public string? Encrypt(string? data)
        {
            if (string.IsNullOrEmpty(data)) return data;

            var key = Encoding.UTF8.GetBytes(_secrets.AesKey);
            var iv = Encoding.UTF8.GetBytes(IVCONST);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.IV = iv;

                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                            csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                        }

                        var encryptedBytes = msEncrypt.ToArray();

                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
        }
    }
}

