using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EmployeeAddressModel
    {
        public int id { get; set; }
        public string? entitystate { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string? addresstype { get; set; }
        public string? streetaddress1 { get; set; }
        public string? streetaddress2 { get; set; }
        public string? streetaddress3 { get; set; }
        public string? streetaddress4 { get; set; }
        public string? streetaddress5 { get; set; }
        public string? streetaddress6 { get; set; }
        public string? city { get; set; }
        public string? county { get; set; }
        public string? state { get; set; }
        public string? postalcode { get; set; }
        public string? country { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
