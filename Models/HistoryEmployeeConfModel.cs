using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class HistoryEmployeeConfModel : HistoryEmployeeConf
    {
        public string? filename { get; set; }
    }
}
