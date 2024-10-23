using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResponseSelectListValueModel
    {
        public int? id { get; set; }
        public int? selectlistid { get; set; }
        public string? tablename { get; set; }
        public string? columnname { get; set; }
        public string? type { get; set; }
        public string? inputvalue { get; set; }
        public string? displayvalue { get; set; }
        public string? outputvalue { get; set; }
        public string? overridelevel { get; set; }
        public int? overrideorder { get; set; }
        public string? rank { get; set; }
    }
}
