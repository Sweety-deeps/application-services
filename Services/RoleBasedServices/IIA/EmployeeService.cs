using Persistence;
using Microsoft.Extensions.Logging;
using Domain.Models.Users;
using Domain.Entities;
using DomainLayer.Entities;
using Services.Helpers;
using Domain.Models;

namespace Services.IIA
{
    public class EmployeeService : Services.EmployeeService
    {
        public EmployeeService(AppDbContext appDbContext, ILogger<EmployeeService> logger, ISelectListHelper selectListHelper) : base(appDbContext, logger, selectListHelper)
        {
        }

        public override List<EmployeeAddress> GetEmpAddress(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmpAddress(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<EmployeeBank> GetEmpBanks(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmpBanks(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<EmployeeConf> GetEmpConf(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmpConf(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override Task<List<EmployeeContrySpecific>> GetEmpCSPs(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmpCSPs(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<EmployeeJob> GetEmpJobs(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmpJobs(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<Employee> GetEmployees(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmployees(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<EmployeePayDeduction> GetEmpPayDs(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmpPayDs(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<EmployeeSalary> GetEmpSalarys(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetEmpSalarys(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeeAddressModel>> GetHistoryEmpAddress(LoggedInUser user, int paygroupId, int id)
        {
            if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmpAddress(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeeBankModel>> GetHistoryEmpBanks(LoggedInUser user, int paygroupId, int id)
        {
           if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmpBanks(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeeConfModel>> GetHistoryEmpConf(LoggedInUser user, int paygroupId, int id)
        {
           if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmpConf(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeeCspfModel>> GetHistoryEmpCSPs(LoggedInUser user, int paygroupId, int id)
        {
           if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmpCSPs(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeeJobModel>> GetHistoryEmpJobs(LoggedInUser user, int paygroupId, int id)
        {
           if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmpJobs(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeeModel>> GetHistoryEmployees(LoggedInUser user, int paygroupId, int id)
        {
           if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmployees(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeePayDeductionModel>> GetHistoryEmpPayDs(LoggedInUser user, int paygroupId, int id)
        {
           if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmpPayDs(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override async Task<List<HistoryEmployeeSalaryModel>> GetHistoryEmpSalarys(LoggedInUser user, int paygroupId, int id)
        {
            if (user.Paygroups.Where(p => p.payGroupId == paygroupId).Any())
            {
                return await base.GetHistoryEmpSalarys(user, paygroupId, id);
            }
            throw new UnauthorizedAccessException();
        }

        public override List<TimeAndAttendance> GetTimeAndAttendance(LoggedInUser user, string paygroupCode)
        {
            if (user.Paygroups.Where(p => p.payGroupCode == paygroupCode).Any())
            {
                return base.GetTimeAndAttendance(user, paygroupCode);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
