namespace Domain.Models.Users
{
    public class PayGroupAssignmentModel
	{
        public double Id { get; set; }
		public string PayGroupCode { get; set; }
        public string ClientName { get; set; }
        public string LegalEntityName { get; set; }
        public int ClientId { get; set; }
        public int LegalEntityId { get; set; }
        public int PaygroupId { get; set; }
        public string Status { get; set; }
    }
}

