using DocumentFormat.OpenXml.InkML;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class SelectListHelper : ISelectListHelper
    {
        private readonly AppDbContext _appDbContext;

        public SelectListHelper(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        private static List<SelectListModel> AssignRoles()
        {
            List<SelectListModel> role = new()
            {
                new() { StoreValue = "superuser", DisplayValue = "Super user", Enabled = true, Order = 0 },
                new() { StoreValue = "poweruser", DisplayValue = "Power user", Enabled = true, Order = 1 },
                new() { StoreValue = "interface_ic", DisplayValue = "Interface Implementation Analyst", Enabled = true, Order = 2 },
                new() { StoreValue = "Interface_oa", DisplayValue = "Interface Operation Analyst", Enabled = true, Order = 3 },
                new() { StoreValue = "country_manager", DisplayValue = "Country Manager", Enabled = true, Order = 4 },
                new() { StoreValue = "CAM", DisplayValue = "CAM", Enabled = true, Order = 5 },
                new() { StoreValue = "document_manager", DisplayValue = "Document Manager User", Enabled = true, Order = 6 }
            };
            
            var sortedRoles = role.OrderBy(x => x.Order).ToList();
            return sortedRoles;
        }

        public List<SelectListModel> GetConf(string storedResult)
        {
            var query = from cp in _appDbContext.countrypicklist
                        where cp.displayvalue == storedResult
                        group cp by cp.id into employeeGroups
                        select new SelectListModel
                        {
                            StoreValue = employeeGroups.First().jsonvalue,
                            DisplayValue = employeeGroups.First().displayvalue,
                        };

            List<SelectListModel> result = query.Distinct().ToList();

            return result;
        }

        public string GetDisplayValue(string storeValue)
        {
            try
            {
                var role = AssignRoles().FirstOrDefault(x => x.StoreValue == storeValue);
                return role.DisplayValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public List<string?> GetCountrySpecificFields(string fieldType, string interfaceName, int countryId)
        {
            var countrySpecificFields = _appDbContext.countryspecificfields
                .Where(cp => cp.FieldType == fieldType && cp.InterfaceName == interfaceName && cp.CountryId == countryId)
                .Select(cp => cp.FieldName)
                .ToList();

            return countrySpecificFields;
        }

        public List<ResponseSelectListValueModel> GetFilteredSelectListValues(string paygroupCode, string interfaceName, string tableNameFilter)
        {
            var selectListQuery = "SELECT * FROM dbo.getselectlistvalues(@PayGroupCode, @InterfaceName)";
            var paygroupCodeParam = new NpgsqlParameter("@PayGroupCode", paygroupCode);
            var interfaceNameParam = new NpgsqlParameter("@InterfaceName", interfaceName);

            var query = _appDbContext.Set<ResponseSelectListValueModel>()
                                     .FromSqlRaw(selectListQuery, paygroupCodeParam, interfaceNameParam).ToList();

            var filteredResults = query.Where(r => r.tablename == tableNameFilter).ToList();

            return filteredResults;
        }

        public static string? GetFieldValue(List<ResponseSelectListValueModel> filteredResults, string? fieldName, string? value, FieldValueType valueType)
        {
            var result = filteredResults.FirstOrDefault(fr =>
                string.Equals(fr.columnname, fieldName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fr.inputvalue, value, StringComparison.OrdinalIgnoreCase)
            );
            return valueType switch
            {
                FieldValueType.DisplayValue => result?.displayvalue ?? value,
                FieldValueType.OutputValue => result?.outputvalue ?? value,
                _ => value 
            };
        }

        public static string GetInterfaceType(string? outboundformat)
        {
            return outboundformat == "JSON" ? "DPI" : "CP";
        }
    }
}
