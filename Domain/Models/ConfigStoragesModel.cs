using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ConfigStoragesModel
    {
        public int id { get; set; }
        public int payrollcompanyid { get; set; }
        public int requesttypeid { get; set; }
        public string? inboundstoragelocationbucket { get; set; }
        public string? inboundstoragelocationfolder { get; set; }
        public string? outboundpestoragelocationbucket { get; set; }
        public string? outboundpestoragelocationfolder { get; set; }
        public string? inboundpestoragelocationbucket { get; set; }
        public string? inboundpestoragelocationfolder { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
