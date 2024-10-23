namespace Domain.Entities
{
    public class Client
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int? providerid { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
