using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Domain.Entities
{
    public class UserPaygroupAssignment : BaseEntity
    {
        [Column("clientid")]
        public int ClientId { get; set; }
        [Column("legalentityid")]
        public int LegalEntityId { get; set; }
        [Column("paygroupid")]
        public int PaygroupId { get; set; }
        [Column("userid")]
        public Guid UserId { get; set; }
        [Column("status")]
        public string Status { get; set; } = "INACTIVE";

    }
}