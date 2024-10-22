using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Dayforce
{
    public class DayforcePostSftpRequest
    {
        public string CountryCode { get; set; } = "ISO 3166-1";
        public string PaygroupCode { get; set; }
        public string LegalentityCode { get; set; }
        public string? SftpFolder { get; set; }
    }
}
