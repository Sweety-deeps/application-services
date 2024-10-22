using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public interface IDateTimeHelper
    {
        DateTime GetDateTimeNow();
        DateTime? GetDateTimeWithTimezone(DateTime? dateTimeInUtc);

        DateTime? SetDateTimeWithTime(DateTime? date, TimeSpan time);
    }
}
