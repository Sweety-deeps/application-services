namespace Domain.Models
{
    public class UserPayGroupModel
    {
        public double? Id { get; set; }
        public int ClientId { get; set; }
        public int LegalEntityId { get; set; }
        public int PaygroupId { get; set; }
        public string Status { get; set; }
    }
}
