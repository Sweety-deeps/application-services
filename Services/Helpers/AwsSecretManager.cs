using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace Services.Helpers
{
    public class AwsSecretManager : ISecretManager
	{
        private readonly IAmazonSecretsManager _awsSecretManagerClient;

        public AwsSecretManager(IAmazonSecretsManager amazonSecretsManager)
		{
            _awsSecretManagerClient = amazonSecretsManager;
		}

        public async Task<Dictionary<string, object>> GetSecrets(string secretId)
        {
            var request = new GetSecretValueRequest()
            {
                SecretId = secretId,
                VersionStage = "AWSCURRENT",
            };

            // do exception handling if needed
            var response = await _awsSecretManagerClient.GetSecretValueAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Secret manager retrieval failed with response status {response.HttpStatusCode} for the secret {secretId}");
            }

            if (!string.IsNullOrWhiteSpace(response.SecretString))
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.SecretString);
                return result ?? new Dictionary<string, object>();
            }
            else
            {
                using var memoryStream = response.SecretBinary;
                using var reader = new StreamReader(memoryStream);

                string resultAsString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultAsString);
                return result ?? new Dictionary<string, object>();
            }
        }
    }
}
