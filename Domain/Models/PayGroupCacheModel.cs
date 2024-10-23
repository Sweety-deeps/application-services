namespace Domain.Models
{
    public class PayGroupCacheModel
    {
        public int payGroupId { get; set; }
        public string payGroupCode { get; set; }
        public string name { get; set; }
        public int countryId { get; set; }
        public int legalEntityId { get; set; }
        public string legalentitycode { get; set; }
        public int clientId { get; set; }
        public string clientCode { get; set; }
        public string? status { get; set; }
        public int payfrequencyid { get; set; }
    }
}
