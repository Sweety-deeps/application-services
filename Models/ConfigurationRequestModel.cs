using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
        public class ConfigurationRequestModel
        {
            public string? paygroup { get; set; }
            public string? configname { get; set; }
            public int? year { get; set; }
        }
        public class ConfigurationResponseModel
        {
            bool status { get; set; }
            public List<PayrollElementsModel>? payelements { get; set; }
            public List<PayCalendarModel>? paycalendar { get; set; }
            public string filename { get; set; }
        //To do: other config list to be added
    }

}
