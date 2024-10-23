using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PayGroup
    {

        public int id { get; set; }
        public string code { get; set; }
        public int legalentityid { get; set; }
        public string name { get; set; }
        public int payfrequencyid { get; set; }
        public int countryid { get; set; }
        public string? emailto { get; set; }
        public string? emailcc { get; set; }
        public string? emailsubject { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string? outboundformat { get; set; }
        public string? transactioncurrency { get; set; }
        public string? inboundformat { get; set; }
        [Column("inbound_sftp_folder")]
        public string? InboundSftpFolder { get; set; }
        [Column("gpri_sftp_folder")]
        public string? GpriSftpFolder { get; set; }
        [Column("payslip_sftp_folder")]
        public string? PayslipSftpFolder { get; set; }
        [Column("cve_cia")]
        public string? cvecia { get; set; }
        public bool collectchanges { get; set; }
        [Column("apiclientid")]
        public string? ApiClientId { get; set; }
        [Column("apiusername")]
        public string? ApiUserName { get; set; }
        [Column("apipassword")]
        public string? ApiPassword { get; set; }
        [Column("urlprefix")]
        public string? urlPrefix { get; set; }
        [Column("outbound_sftp_server")]
        public string? OutboundSftpServer { get; set; }
    }
}
