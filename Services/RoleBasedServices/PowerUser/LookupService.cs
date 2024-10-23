using System.Reflection;
using DocumentFormat.OpenXml.Office.CustomUI;
using Domain.Models;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Services.Abstractions;

namespace Services.PowerUser
{
    public class LookupService : Services.LookupService
    {
        private readonly Dictionary<string, IPayGroupServices> _paygroupService;
        private readonly Dictionary<string, ILegalEntityServices> _legalEntityService;
        private readonly Dictionary<string, IClientServices> _clientService;
        private readonly ILoggedInUserRoleService _loggedInUserRoleService;

        public LookupService(AppDbContext appDbContext, IEnumerable<IPayGroupServices> paygroupService, IEnumerable<ILegalEntityServices> legalEntityService, IEnumerable<IClientServices> clientService, ILoggedInUserRoleService loggedInUserRoleService) : base(appDbContext)
        {
            _paygroupService = paygroupService.ToDictionary(service => service.GetType().Namespace);
            _legalEntityService = legalEntityService.ToDictionary(service => service.GetType().Namespace); ;
            _clientService = clientService.ToDictionary(service => service.GetType().Namespace); ;
            _loggedInUserRoleService = loggedInUserRoleService;
        }

        public override List<LookupValue> GetLookupFields(LoggedInUser user, int id, string? value)
        {
            var lId = (Lookups)Enum.Parse(typeof(Lookups), id.ToString());
            lookupDefinitions.TryGetValue(lId, out Lookup? lookup);
            var lookupValues = new List<LookupValue>();
            if (lookup == null)
                return lookupValues;
            var lookupColName = "<" + lookup.FilterColumn.ToUpper() + ">K__BACKINGFIELD";
            var selectColName = "<" + lookup.ColumnName.ToUpper() + ">K__BACKINGFIELD";

            switch (lId)
            {
                case Lookups.Client:
                    IClientServices clientService = _loggedInUserRoleService.GetServiceForController<IClientServices>(user, _clientService);
                    var clientResult = clientService.GetClient(user);
                    Type clientClassType = typeof(ClientModel);
                    FieldInfo[] clientFields = clientClassType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var clientFilterField = clientFields.Where(field => field.Name.ToUpper() == lookupColName).FirstOrDefault();
                    var clientSelectField = clientFields.Where(field => field.Name.ToUpper() == selectColName).FirstOrDefault();
                    lookupValues = clientResult.Result
                                    .Where(item => value == null || clientFilterField == null ? true : clientFilterField.GetValue(item).ToString().Equals(value))
                                    .Select(x => new LookupValue { StoreValue = x.id, DisplayValue = clientSelectField.GetValue(x).ToString() })
                                    .ToList();
                    break;

                case Lookups.LegalEntity:
                    ILegalEntityServices leService = _loggedInUserRoleService.GetServiceForController<ILegalEntityServices>(user, _legalEntityService);
                    var result = leService.GetLegalEntityDetails(user);
                    Type leClassType = typeof(LegalEntityModel);
                    FieldInfo[] leFields = leClassType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var leFilterField = leFields.Where(field => field.Name.ToUpper() == lookupColName).FirstOrDefault();
                    var leSelectField = leFields.Where(field => field.Name.ToUpper() == selectColName).FirstOrDefault();
                    lookupValues = result.Result
                                    .Where(item => value == null || leFilterField == null ? true : leFilterField.GetValue(item).ToString().Equals(value))
                                    .Select(x => new LookupValue { StoreValue = x.id, DisplayValue = leSelectField.GetValue(x).ToString() })
                                    .ToList();
                    break;

                case Lookups.Paygroup:
                    IPayGroupServices pgservice = _loggedInUserRoleService.GetServiceForController<IPayGroupServices>(user, _paygroupService);
                    var payGroupResult = pgservice.GetPayGroupDetails(user);
                    Type payGroupClass = typeof(PayGroupModel);
                    FieldInfo[] pgFields = payGroupClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var pgFilterField = pgFields.Where(field => field.Name.ToUpper() == lookupColName).FirstOrDefault();
                    var pgSelectField = pgFields.Where(field => field.Name.ToUpper() == selectColName).FirstOrDefault();
                    lookupValues = payGroupResult.Result
                                    .Where(item => value == null || pgFilterField == null ? true : pgFilterField.GetValue(item).ToString().Equals(value))
                                    .Select(x => new LookupValue { StoreValue = x.id, DisplayValue = pgSelectField.GetValue(x).ToString() })
                                    .ToList();
                    break;
            }


            return lookupValues;
        }
    }
}

