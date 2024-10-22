namespace Services.Helpers
{
    public class PasswordHelper
	{
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Symbol = "!-_*+&$#@')(%";
        private const string Number = "0123456789";

        private static readonly Random Random = new();

        public static string CreateRandomPassword(int numberOfCharacters)
        {
            return Create(numberOfCharacters, Uppercase, Lowercase, Symbol, Number);
        }

        private static string Create(int length, params string[] keys)
        {
            var chars = new char[length];
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                chars[i] = key[Random.Next(key.Length)];
            }

            for (int i = keys.Length; i < length; i++)
            {
                var indexKeys = Random.Next(keys.Length);
                var key = keys[indexKeys];
                chars[i] = key[Random.Next(key.Length)];
            }

            return new string(chars.OrderBy(x => Guid.NewGuid()).ToArray());
        }
    }
}
