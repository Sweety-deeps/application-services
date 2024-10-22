using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Services.Abstractions;

namespace Services
{
    public class LookupService : ILookupService
    {
        protected readonly AppDbContext _appDbContext;

        public LookupService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public static readonly Dictionary<Lookups, Lookup> lookupDefinitions = new Dictionary<Lookups, Lookup>
        {
            { Lookups.Client, new Lookup {Id = 1, TableName = "dbo.client", ColumnName = "name", FilterColumn = "" } },
            { Lookups.LegalEntity, new Lookup {Id = 2, TableName = "dbo.legalentity", ColumnName = "name", FilterColumn = "clientid" } },
            { Lookups.Paygroup, new Lookup {Id = 3, TableName = "dbo.paygroup", ColumnName = "code", FilterColumn = "legalentityid" } },
        };

        public virtual List<LookupValue> GetLookupFields(LoggedInUser user, int id, String? value)
        {
            var lId = (Lookups) Enum.Parse(typeof(Lookups), id.ToString());
            lookupDefinitions.TryGetValue(lId, out Lookup? lookup);
            var lookupValues = new List<LookupValue>();
            if(lookup == null)
                return lookupValues;

            var query = $"SELECT id, {lookup.ColumnName} FROM {lookup.TableName}" +
                (lookup.FilterColumn != null && value != null ? $" WHERE {lookup.FilterColumn} = {value}" : "");

            using (var command = _appDbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                _appDbContext.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                        lookupValues.Add(new LookupValue {
                            StoreValue = result.GetInt32(0),
                            DisplayValue = result.GetString(1)
                        });
                }

                _appDbContext.Database.CloseConnection();
            }

            return lookupValues;
        }
    }
}

