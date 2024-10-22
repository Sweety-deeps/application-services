using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Domain.Entities.Users;
using Domain.Models.Users;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.Helpers;

namespace Services.Users
{
    public class CognitoService : ICognitoService
	{
        private readonly ILogger<CognitoService> _logger;
		private readonly AwsCognito _awsCognitoConfig;
        private readonly IAmazonCognitoIdentityProvider _cognitoProvider;

        public CognitoService(AwsCognito awsCognito,
			IAmazonCognitoIdentityProvider cognitoProvider,
            ILogger<CognitoService> logger)
		{
			_awsCognitoConfig = awsCognito;
			_cognitoProvider = cognitoProvider;
            _logger = logger;
        }

		public async Task<UserType> CreateUser(User user)
		{
            _logger.LogInformation("Cognito details User pool id, {userPoolId} ClientId, {appClientId}", _awsCognitoConfig.UserPoolId, _awsCognitoConfig.AppClientId);
			var userRequest = new AdminCreateUserRequest()
			{
                UserPoolId = _awsCognitoConfig.UserPoolId,
                DesiredDeliveryMediums = new List<string>
				{
					"EMAIL"
				},
                TemporaryPassword = _awsCognitoConfig.DefaultPassword ? "Test@2023" : PasswordHelper.CreateRandomPassword(12),
                UserAttributes = CreateUserAttributesList(user),
                Username = user.UserId,
            };
			var response = await _cognitoProvider.AdminCreateUserAsync(userRequest);

            _logger.LogDebug("Cognito service rsponded with {responseStatus}", response.HttpStatusCode);


            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.User;
            } else
            {
                throw new Exception($"Exception occurred while creating the user, Create user failed with {response.HttpStatusCode}");
            }
		}

        public async Task DeleteUser(string username)
        {
            var request = new AdminDeleteUserRequest()
            {
                UserPoolId = _awsCognitoConfig.UserPoolId,
                Username = username,
            };

            var response = await _cognitoProvider.AdminDeleteUserAsync(request);

            _logger.LogDebug("Cognito user deletion responded with {responseStatus}", response.HttpStatusCode);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Exception occurred while creating the user, Create user failed with {response.HttpStatusCode}");
            }
        }

        public async Task UpdateUser(String userId, User user)
        {
            var awsUser = JsonConvert.DeserializeObject<AwsUser>(user.AdditionalData);
            bool isSuccess;
            if (user.Status == "ACTIVE")
            {
                isSuccess = await EnableUser(awsUser.Username);
            } else
            {
                isSuccess = await DisableUser(awsUser.Username);
            }

            if (!isSuccess)
            {
                throw new Exception($"Exception occurred while updating the user, please check cognito logs");
            }

			var userRequest = new AdminUpdateUserAttributesRequest()
			{
                UserPoolId = _awsCognitoConfig.UserPoolId,
                UserAttributes = CreateUserAttributesList(user),
                Username = userId,
            };
			var response = await _cognitoProvider.AdminUpdateUserAttributesAsync(userRequest);

            _logger.LogDebug("Cognito service responded with {responseStatus}", response.HttpStatusCode);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK) {
                throw new Exception($"Exception occurred while updating the user, Update user failed with {response.HttpStatusCode}");
            }
        }

        private async Task<bool> DisableUser(string username)
        {
            var request = new AdminDisableUserRequest()
            {
                UserPoolId = _awsCognitoConfig.UserPoolId,
                Username = username,
            };
            var response = await _cognitoProvider.AdminDisableUserAsync(request);
            _logger.LogDebug("Cognito user deletion responded with {responseStatus}", response.HttpStatusCode);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        private async Task<bool> EnableUser(string username)
        {
            var request = new AdminEnableUserRequest()
            {
                UserPoolId = _awsCognitoConfig.UserPoolId,
                Username = username,
            };
            var response = await _cognitoProvider.AdminEnableUserAsync(request);
            _logger.LogDebug("Cognito user deletion responded with {responseStatus}", response.HttpStatusCode);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;

        }

        private List<AttributeType> CreateUserAttributesList(User user) {
            var userAttributes = new List<AttributeType> {
                new AttributeType {
                    Name = "given_name",
                    Value = user.FirstName ?? string.Empty,
                },
                new AttributeType {
                    Name = "family_name",
                    Value = user.LastName ?? string.Empty,
                },
                new AttributeType
                {
                    Name = "name",
                    Value = user.FullName ?? string.Empty,
                },
                new AttributeType
                {
                    Name = "custom:role",
                    Value = user.Role ?? string.Empty,
                },
                new AttributeType
                {
                    Name = "custom:usergroup",
                    Value = user.UserGroup ?? string.Empty,
                },
                new AttributeType {
                    Name = "email",
                    Value = user.Email ?? string.Empty,
                },
                new AttributeType
                {
                    Name = "preferred_username",
                    Value = user.UserId ?? string.Empty,
                },
                new AttributeType {
                    Name = "email_verified",
                    Value = "true",
                },
            };

            return userAttributes;
        }
    }
}

