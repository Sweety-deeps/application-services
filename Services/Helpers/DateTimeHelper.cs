using Domain.Models;
using TimeZoneConverter;

namespace Services.Helpers
{
    public class DateTimeHelper : IDateTimeHelper
    {
        private readonly Config _config;
        private readonly TimeZoneInfo _tzi;
        public DateTimeHelper(Config config)
        {
            _config = config;
            _tzi = TZConvert.GetTimeZoneInfo(_config.DefaultTimeZone);
        }


        public DateTime GetDateTimeNow()
        {
            return _config.UtcEnabled ? DateTime.UtcNow : DateTime.Now;
        }

        public DateTime? GetDateTimeWithTimezone(DateTime? dateTimeInUtc)
        {
            return dateTimeInUtc == null ? null : TimeZoneInfo.ConvertTimeFromUtc(dateTimeInUtc.Value, _tzi);
        }
        public DateTime? SetDateTimeWithTime(DateTime? date, TimeSpan time)
        {
            if (date.HasValue && date.Value.TimeOfDay == TimeSpan.Zero)
            {
                return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, time.Hours, time.Minutes, time.Seconds);
            }
            return date;
        }
    }   
}
