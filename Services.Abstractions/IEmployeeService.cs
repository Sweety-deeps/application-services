using Domain.Entities;
using Domain.Models;
using Domain.Models.Users;
using DomainLayer.Entities;

namespace Services.Abstractions
{
    public interface IEmployeeService : IUIPermissions
    {
        List<Employee> GetEmployees(LoggedInUser user, string paygroupCode);
        List<EmployeeJob> GetEmpJobs(LoggedInUser user, string paygroupCode);
        List<EmployeeAddress> GetEmpAddress(LoggedInUser user, string paygroupCode);
        List<EmployeeBank> GetEmpBanks(LoggedInUser user, string paygroupCode);
        List<EmployeeSalary> GetEmpSalarys(LoggedInUser user, string paygroupCode);
        List<EmployeePayDeduction> GetEmpPayDs(LoggedInUser user, string paygroupCode);
        Task<List<EmployeeContrySpecific>> GetEmpCSPs(LoggedInUser user, string paygroupCode);
        List<EmployeeConf> GetEmpConf(LoggedInUser user, string paygroupCode);
        List<TimeAndAttendance> GetTimeAndAttendance(LoggedInUser user, string paygroupCode);
        Task<List<HistoryEmployeeModel>> GetHistoryEmployees(LoggedInUser user, int paygroupId, int id);
        Task<List<HistoryEmployeeJobModel>> GetHistoryEmpJobs(LoggedInUser user, int paygroupId, int id);
        Task<List<HistoryEmployeeAddressModel>> GetHistoryEmpAddress(LoggedInUser user, int paygroupId, int id);
        Task<List<HistoryEmployeeBankModel>> GetHistoryEmpBanks(LoggedInUser user, int paygroupId, int id);
        Task<List<HistoryEmployeeSalaryModel>> GetHistoryEmpSalarys(LoggedInUser user, int paygroupId, int id);
        Task<List<HistoryEmployeePayDeductionModel>> GetHistoryEmpPayDs(LoggedInUser user, int paygroupId, int id);
        Task<List<HistoryEmployeeCspfModel>> GetHistoryEmpCSPs(LoggedInUser user, int paygroupId, int id);
        Task<List<HistoryEmployeeConfModel>> GetHistoryEmpConf(LoggedInUser user, int paygroupId, int id);
    }
}
