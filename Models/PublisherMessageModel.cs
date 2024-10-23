using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PublisherMessageModel
    { 
        public int entityId { get; set; }
        public string entityName { get; set; }
        public string? template { get; set; }
    }
}
