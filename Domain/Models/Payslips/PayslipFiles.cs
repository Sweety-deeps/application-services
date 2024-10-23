namespace Domain.Models.Payslips
{
    public class PayslipFiles
	{
        public int id { get; set; }
        public string filename { get; set; }
        public string empid { get; set; }
        public string payperiod { get; set; }
        public string s3objectkey { get; set; }
    }
}
