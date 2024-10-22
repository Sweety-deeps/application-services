namespace Services.Helpers
{
    public interface IEncrytionHelper
    {
        public string? Encrypt(string? data);
        public string? Decrypt(string? encryptedValue);
    }
}