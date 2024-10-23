using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
	public abstract class BaseEntity
	{
		public BaseEntity()
		{
		}

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
		public double Id { get; set; }
        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("modifiedby")]
        public string ModifiedBy { get; set; }
        [Column("modifiedat")]
        public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}

