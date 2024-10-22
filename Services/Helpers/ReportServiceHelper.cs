using Amazon.CognitoIdentityProvider.Model;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Domain.Entities;
using Domain.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class ReportServiceHelper: IReportServiceHelper
    {
        private readonly AppDbContext _appDbContext;

        public ReportServiceHelper(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public virtual async Task<string> GetClientCodeByPayGroup(string? paygroup)
        {
            if (string.IsNullOrEmpty(paygroup))
            {
                return string.Empty;
            }
            try
            {
                var clientcode = await (from p in _appDbContext.Set<PayGroup>()
                                        join l in _appDbContext.Set<LegalEntity>() on p.legalentityid equals l.id
                                        join c in _appDbContext.Set<Client>() on l.clientid equals c.id
                                        where p.code == paygroup
                                        select c.code).FirstOrDefaultAsync();

                return clientcode;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public virtual string GenerateSheetName(string? paygroup, string reportSuffix)
        {
            var dateString = DateTime.Now.ToString("yyyyMMdd");
            if (string.IsNullOrEmpty(paygroup))
            {
                paygroup = "All Paygroups";
            }
            if(paygroup.Length > 13)
                paygroup = paygroup.Substring(0, 13);

            string sheetName = $"{paygroup}_{reportSuffix}_{dateString}";


            if (sheetName.Length > 31)
            { 
                sheetName = $"{paygroup}_{reportSuffix}_{dateString}";
                sheetName = sheetName.Substring(0, 31);
            }

            return sheetName;
        }
        public virtual string GenerateSheetNameForPC(string? paygroup, int? year, string reportSuffix)
        {
            var reportyear = year.ToString();

            if (string.IsNullOrEmpty(paygroup))
            {
                paygroup = "All Paygroups";
            }
            if (paygroup.Length > 13)
                paygroup = paygroup.Substring(0, 13);

            string sheetName = $"{paygroup}_{reportyear}_{reportSuffix}";


            if (sheetName.Length > 31)
            {
                sheetName = $"{reportSuffix}";
                sheetName = sheetName.Substring(0, 31);
            }

            return sheetName;
        }
        public virtual string? GetInterfaceType(int? paygroupid)
        {
            if (paygroupid.HasValue)
            {
                var interfacetype = (from p in _appDbContext.Set<PayGroup>()
                                     where p.id == paygroupid
                                     select p.outboundformat).FirstOrDefault();
                return interfacetype;
            }
            return null;
        }
    }
}
