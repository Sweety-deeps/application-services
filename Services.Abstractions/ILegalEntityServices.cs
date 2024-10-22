using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface ILegalEntityServices : IUIPermissions
    {
        Task<List<LegalEntityModel>> GetLegalEntityDetails(LoggedInUser user);
        Task<DatabaseResponse> InsertLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel);
        Task<DatabaseResponse> UpdateLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel);
        Task<DatabaseResponse> DeleteLegalEntity(LoggedInUser user, int id);
        Task<List<LegalEntityBaseModel>> GetLegalEntityBaseDetails(LoggedInUser user);
    }
}
