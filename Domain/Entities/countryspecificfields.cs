using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class countryspecificfields 
    {
        [Column("id")]
        public double? Id { get; set; }

        [Column("countryid")]
        public int CountryId { get; set; }

        [Column("interfacename")]
        public string? InterfaceName { get; set; }

        [Column("fieldname")]
        public string? FieldName { get; set; }

        [Column("fieldtype")]
        public string? FieldType { get; set; }

        [Column("createdat")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("modifiedby")]
        public string? ModifiedBy { get; set; }

        [Column("modifiedat")]
        public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
