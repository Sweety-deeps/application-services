using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IPayrollElementServices : IUIPermissions
    {
        List<PayrollElementsModel> GetPayrollElements(LoggedInUser user, string paygroupCode);
        DatabaseResponse InsertPayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel);
        DatabaseResponse UpdatePayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel);
        void DeletePayrollElements(LoggedInUser user, int id);
        Task<DatabaseResponse> UploadPayElements(LoggedInUser user, PayCalendarUploadModal mdl);

    }
}
