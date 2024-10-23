using Domain.Entities;
using Domain.Entities.Users;

namespace Domain.Models.Users.Extensions
{
    public static class UserExtension
	{
		public static UserResponseModel ToUserResponseModel(this User user)
		{
			return new UserResponseModel()
			{
				Id = user.Id,
				UserId = user.UserId,
				Email = user.Email,
				Role = user.Role,
				UserGroup = user.UserGroup,
				FirstName = user.FirstName,
				LastName = user.LastName,
				MiddleName = user.MiddleName,
				SecondLastName = user.SecondLastName,
				FullName = user.FullName,
				Status = user.Status,
                LastLoggedOn = user.LastLoggedOn
            };
		}

		public static UserResponseModel ToUserResponseModel(this User user, List<PayGroupAssignmentModel> paygroupAssignments)
		{
			var userResponseModel = user.ToUserResponseModel();
			userResponseModel.PaygroupsAssigned = paygroupAssignments;

			return userResponseModel;
		}
	}
}

