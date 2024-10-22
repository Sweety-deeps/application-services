using Services.Abstractions;
using Domain.Models;
using Domain.Models.Users;

namespace Services.UnAuthorized
{
    public class PayrollElementServices : IPayrollElementServices
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

        public void DeletePayrollElements(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PayrollElementsModel> GetPayrollElements(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse InsertPayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse UpdatePayrollElements(LoggedInUser user, PayrollElementsModel payrollelementsModel)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> UploadPayElements(LoggedInUser user, PayCalendarUploadModal mdl)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
