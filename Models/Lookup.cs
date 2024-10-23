namespace Domain.Models
{
    public class Lookup
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string FilterColumn { get; set; }
    }

    public enum Lookups
    {
        Client = 1,
        LegalEntity = 2,
        Paygroup = 3
    }
}
