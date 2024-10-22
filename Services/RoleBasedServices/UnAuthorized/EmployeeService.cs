using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using DomainLayer.Entities;
using Services.Abstractions;

namespace Services.UnAuthorized
{
    public class EmployeeService : IEmployeeService
    {
        public virtual bool CanView(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanEdit(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanAdd(LoggedInUser user)
        {
            return false;
        }

        public virtual bool CanDelete(LoggedInUser user)
        {
            return false;
        }

        public List<EmployeeAddress> GetEmpAddress(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<EmployeeBank> GetEmpBanks(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<EmployeeConf> GetEmpConf(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<EmployeeContrySpecific>> GetEmpCSPs(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<EmployeeJob> GetEmpJobs(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<Employee> GetEmployees(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<EmployeePayDeduction> GetEmpPayDs(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public List<EmployeeSalary> GetEmpSalarys(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeeAddressModel>> GetHistoryEmpAddress(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeeBankModel>> GetHistoryEmpBanks(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeeConfModel>> GetHistoryEmpConf(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeeCspfModel>> GetHistoryEmpCSPs(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeeJobModel>> GetHistoryEmpJobs(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeeModel>> GetHistoryEmployees(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeePayDeductionModel>> GetHistoryEmpPayDs(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public Task<List<HistoryEmployeeSalaryModel>> GetHistoryEmpSalarys(LoggedInUser user, int paygroupId, int id)
        {
            throw new UnauthorizedAccessException();
        }

        public List<TimeAndAttendance> GetTimeAndAttendance(LoggedInUser user, string paygroupCode)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
