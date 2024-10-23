using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class HistoryEmployeeModel : HistoryEmployee
    {
        public string? filename { get; set; }
    }
}
