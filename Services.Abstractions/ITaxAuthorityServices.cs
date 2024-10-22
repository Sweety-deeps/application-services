using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface ITaxAuthorityServices : IUIPermissions
    {
        List<TaxAuthorityModel> GetTaxAuthority(LoggedInUser user);
        DatabaseResponse InsertTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel);
        Task<DatabaseResponse> UpdateTaxAuthority(LoggedInUser user, TaxAuthorityModel taxAuthorityModel);
        DatabaseResponse DeleteTaxAuthority(LoggedInUser user, int id);

    }
}
