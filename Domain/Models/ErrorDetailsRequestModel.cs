using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ErrorDetailsRequestModel
    {
        public Guid? guid { get; set; }    
        public int? entityId { get; set; } 
        public string? paygroupcode { get; set; }

        public string? entityname { get; set;}

    }
}
