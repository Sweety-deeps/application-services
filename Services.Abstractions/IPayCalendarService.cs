using Domain.Models;
using Domain.Models.Users;

namespace Services.Abstractions
{
    public interface IPayCalendarService : IUIPermissions
    {
        //void AddPayCalendar(LoggedInUser user, PayCalendarModel model);
        DatabaseResponse AddPayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel);
        DatabaseResponse UpdatePayCalendar(LoggedInUser user, PayCalendarModel payCalendarModel);
        List<PayCalendarModel> GetPayCalendar(LoggedInUser user, string paygroupCode);
        void DeletePayCalendar(LoggedInUser user, int id);
        Task<DatabaseResponse> UploadPayCalender(LoggedInUser user, PayCalendarUploadModal mdl);
        List<PayPeriods> GetPayPeriods(LoggedInUser user, int payGroupID);
        List<int> GetPayPeriodsByYear(LoggedInUser user, int payGroupID, int year);
        Task<List<int>> GetPayGroupCalendarYears (int id);
    }
}
