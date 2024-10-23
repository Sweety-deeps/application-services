namespace Domain.Entities
{
    public class GPRIPayRunImports
    {
        public int id { get; set; }
        public string fileid { get; set; }
        public int requestid { get; set; }
        public DateTime? paydate { get; set; }
        public int payperiod { get; set; }
        public int isoffcycle { get; set; }
        public int nooverride { get; set; }
        public string? offcyclepayrunxrefcode { get; set; }
        public string? offcyclepayrundefxrefcode { get; set; }
        public string? offcyclepayruntypexrefcode { get; set; }
        public int payperiodsuffix { get; set; }
        public DateTime? commitdate { get; set; }
        public string? paygroupxrefcode { get; set; }
        public DateTime? payperiodstart { get; set; }
        public DateTime? payperiodend { get; set; }
        public string? employeexrefcode { get; set; }
        public string? legalentityxrefcode { get; set; }
        public string? itemtype { get; set; }
        public string? itemcode { get; set; }
        public decimal itemamount { get; set; }
        public decimal units { get; set; }
        public decimal rate { get; set; }
        public decimal contributetonetpay { get; set; }
        public int ispretax { get; set; }
        public int isemployerdeduction { get; set; }
        public int isemployertax { get; set; }
        public int checkorder { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string? netpay { get; set; }
        public string? localpaycode { get; set; }

    }
}
