using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CalendarRequestModel
    {
        public string? clientname { get; set; }
        public string? paygroup { get; set; }
        public int? year { get; set; }
    }

    public class CalendarResponseModel
    {
        public string? Month { get; set; }
        public int? PayPeriod { get; set; }
        public string? TaskName { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? OffSetStartDate { get; set; }
        public DateTime? OffSetEndDate { get; set; }
        public DateTime? PayDate { get; set; }
    }
    public class CalendarReport
    {
        public string? Months { get; set; }
        public string? PayPeriod { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? OffSetStartDate { get; set; }
        public DateTime? OffSetEndDate { get; set; }
        public DateTime? PayDate { get; set; }
    }


}
