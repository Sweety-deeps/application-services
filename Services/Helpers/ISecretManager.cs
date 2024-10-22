namespace Services.Helpers
{
    public interface ISecretManager
	{

        Task<Dictionary<string, object>> GetSecrets(string secretname);
	}
}
