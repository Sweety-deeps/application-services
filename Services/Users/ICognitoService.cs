using Amazon.CognitoIdentityProvider.Model;
using Domain.Entities.Users;

namespace Services.Users
{
    public interface ICognitoService
	{
		Task<UserType> CreateUser(User user);
		Task DeleteUser(string username);
        Task UpdateUser(String userId, User user);
    }
}

