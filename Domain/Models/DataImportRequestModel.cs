using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class DataImportRequestModel
    {
        public int id { get; set; }
        public int entityID { get; set; }
        public string entityName { get; set; }
        public string template { get; set; }
        public string file { get; set; }
        public string payGroup { get; set; }
    }
}
