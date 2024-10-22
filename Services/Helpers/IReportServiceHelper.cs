using Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public interface IReportServiceHelper
    {
        Task<string> GetClientCodeByPayGroup(string? paygroup);
        string GetInterfaceType(int? paygroupId);
        string GenerateSheetName(string? paygroup, string reportSuffix);
        string GenerateSheetNameForPC(string? paygroup, int? year, string reportSuffix);
    }
}
