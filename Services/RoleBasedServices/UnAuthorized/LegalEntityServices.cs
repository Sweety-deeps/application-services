using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class LegalEntityServices : ILegalEntityServices
    {
        public virtual bool CanView(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public Task<DatabaseResponse> DeleteLegalEntity(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<LegalEntityBaseModel>> GetLegalEntityBaseDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<LegalEntityModel>> GetLegalEntityDetails(LoggedInUser user)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> InsertLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> UpdateLegalEntity(LoggedInUser user, LegalEntityModel legalEntityModel)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
