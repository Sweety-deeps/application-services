using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Domain.Models
{
    public class PayGroupModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int payfrequencyid { get; set; }
        public string payfrequencycode { get; set; }
        public string countrycode { get; set; }
        public int legalentityid { get; set; }
        public int countryid { get; set; }
        public string? emailto { get; set; }
        public string? emailcc { get; set; }
        public string? emailsubject { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string legalentitycode { get; set; }
        public string outboundformat {  get; set; }
        public string? inbound_sftp_folder { get; set; }
        public string? gpri_sftp_folder {  get; set; }
        public string? payslip_sftp_folder { get; set; }
        public string? cvecia { get; set; }
        public string? transactioncurrency { get; set; }
        public string collectchanges { get; set; }
        public string? ApiClientId { get; set; }
        public string? ApiUserName { get; set; }
        public string? ApiPassword { get; set; }
        public string? urlPrefix { get; set; }
        public string? outbound_sftp_server { get; set; }
    }

    public class PaygroupMinimalModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<int> Years { get; set; }
    }
}
