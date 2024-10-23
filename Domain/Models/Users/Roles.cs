using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Users
{
	public enum Role
	{
		document_manager,
		CAM,
		country_manager,
		Interface_oa,
		interface_ic,
		poweruser,
		superuser,
		unauthorized
	}
}

