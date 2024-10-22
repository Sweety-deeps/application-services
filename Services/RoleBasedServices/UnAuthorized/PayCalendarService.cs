using Domain.Models;
using Domain.Models.Users;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class PayCalendarService : IPayCalendarService
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

        public DatabaseResponse AddPayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            throw new UnauthorizedAccessException();
        }

        public void DeletePayCalendar(LoggedInUser user, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PayCalendarModel> GetPayCalendar(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<PayPeriods> GetPayPeriods(LoggedInUser user, int payGroupID)
        {
            throw new UnauthorizedAccessException();
        }

        public List<int> GetPayPeriodsByYear(LoggedInUser user, int payGroupID, int year)
        {
            throw new UnauthorizedAccessException();
        }

        public DatabaseResponse UpdatePayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<DatabaseResponse> UploadPayCalender(LoggedInUser user, PayCalendarUploadModal mdl)
        {
            throw new UnauthorizedAccessException();
        }
        public Task<List<int>> GetPayGroupCalendarYears(int id)
        { 
            throw new UnauthorizedAccessException();
        }
    }
}