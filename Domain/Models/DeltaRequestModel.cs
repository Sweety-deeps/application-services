namespace Domain.Models
{
    public class DeltaRequestModel
    {
        public string paygroup { get; set; }
        public string filterby { get; set; }
        public int? year { get; set; }
        public int? payperiod { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
    }
}
