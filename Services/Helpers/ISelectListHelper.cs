using Domain.Entities;
using Domain.Models;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public interface ISelectListHelper
    {   
        public string GetDisplayValue(string storeValue);
        public List<SelectListModel> GetConf(string storedResult);
        public List<string?> GetCountrySpecificFields(string fieldType, string interfaceName, int countryId);
        public List<ResponseSelectListValueModel> GetFilteredSelectListValues(string paygroupCode, string interfaceName, string tableNameFilter);
    }
}
