using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HistoryEmployeeBank
    {
        public int id { get; set; }
        public int banktableid { get; set; }
        public string? entitystate { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string? bankname { get; set; }
        public string? banknumber { get; set; }
        public string? accounttype { get; set; }
        public string? accountnumber { get; set; }
        public string? ibancode { get; set; }
        public string? swiftcode { get; set; }
        public string? localclearingcode { get; set; }
        public string? beneficiaryname { get; set; }
        //public string? comments { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public bool changeprocessed { get; set; }
        public int? amountorpercentage { get; set; }
        public int? priority { get; set; }
        public string? comments { get; set; }
        public string? banksecondaryid { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set;}
        public string? address3 { get; set;}
        public string? city { get; set; }
        public string? stateprovincecanton { get; set; }
        public string? postalcode { get; set; }
        public string? countrycode { get; set; } 
        public string? splitbankingtype { get; set; }
        public string? fundingmethod { get; set; }
        public int? requestid { get; set; }

    }
}
