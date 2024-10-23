using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SelectListModel
    {
        public string StoreValue { get; set; }
        public string DisplayValue { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }
    }
}
