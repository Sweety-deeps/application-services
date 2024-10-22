using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class CommonServiceHelper
    {
        public static string ExtractEntityName(string? additionalInfo)
        {
            if (string.IsNullOrEmpty(additionalInfo))
            {
                return null;
            }

            var json = JObject.Parse(additionalInfo);
            return json["EntityName"]?.ToString();
        }


        public static string? DiffInTime(DateTime date1, DateTime date2)
        {
            double ProcessingTime = date2.Subtract(date1).TotalSeconds;
            if (ProcessingTime < 0)
            {
                ProcessingTime = date2.AddHours(12).Subtract(date1).TotalSeconds;
            }
            int hours = (int)(ProcessingTime / 3600);
            int minutes = (int)(ProcessingTime % 3600 / 60);
            int seconds = (int)(ProcessingTime % 60);

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";

        }
    }
}
