using Domain.Entities;
using Domain.Entities.Users;
using Domain.Models;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) {
            this.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportServiceDetails>()
            .Property(e => e.ReportFilter)
            .HasColumnType("jsonb");

            modelBuilder.Entity<PeriodChangePERS>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeJDET>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeSLRY>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeBANK>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeADDR>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeCSPF>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeCONF>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangePAYD>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeTIME>().HasNoKey().ToView(null);
            //modelBuilder.Entity<CenamHrBase>().HasNoKey().ToView(null);
            //modelBuilder.Entity<CenamHrCRI>().HasNoKey().ToView(null);
            //modelBuilder.Entity<CenamHrDOM>().HasNoKey().ToView(null);
            //modelBuilder.Entity<CenamHrGTM>().HasNoKey().ToView(null);
            //modelBuilder.Entity<CenamHrPAN>().HasNoKey().ToView(null);
            //modelBuilder.Entity<CenamHrSLV>().HasNoKey().ToView(null);
            //modelBuilder.Entity<CenamPayd>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeStarters>().HasNoKey().ToView(null);
            modelBuilder.Entity<PeriodChangeLeavers>().HasNoKey().ToView(null);
            modelBuilder.Entity<TransactionResponseModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<PayPeriodRegistorDBValuesDetail>().HasNoKey().ToView(null);
            modelBuilder.Entity<PayPeriodWithPrevioudCutOff>().HasNoKey().ToView(null);
            modelBuilder.Entity<GPRI>().Property(x => x.sendgpriresult).HasColumnType("jsonb");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<DataImport> dataimport { get; set; }
        public DbSet<RequestDetails> requestdetails { get; set; }
        public DbSet<ConfigRequestType> configrequesttype { get; set; }
        public DbSet<ConfigStorage> configstorage { get; set; }
        public DbSet<ErrorDetails> errordetails { get; set; }
        public DbSet<Employee> employee { get; set; }
        public DbSet<HistoryEmployee> historyemployee { get; set; }
        public DbSet<StagingEmployee> stagingEmployees { get; set; }
        public DbSet<TransformedEmployee> transformedEmployees { get; set; }
        public DbSet<RequestHighLevelDetails> requesthighleveldetails { get; set; }
        public DbSet<RequestLowLevelDetails> requestlowleveldetails { get; set; }
        public DbSet<ErrorDetails> errorDetails { get; set; }
        public DbSet<EmployeeJob> employeejob { get; set; }
        public DbSet<HistoryEmployeeJob> historyemployeejob { get; set; }
        public DbSet<StagingEmployeeJob> stagingEmployeeJob { get; set; }
        public DbSet<TransformedEmployeeJob> transformedEmployeeJob { get; set; }
        public DbSet<EmployeeAddress> employeeaddress { get; set; }
        public DbSet<HistoryEmployeeAddress> historyemployeeaddress { get; set; }
        public DbSet<StagingEmployeeAddress> stagingEmployeeAddress { get; set; }
        public DbSet<TransformedEmployeeAddress> transformedEmployeeAddress { get; set; }
        public DbSet<EmployeeBank> employeebank { get; set; }
        public DbSet<HistoryEmployeeBank> historyemployeebank { get; set; }
        public DbSet<StagingEmployeeBank> stagingEmployeeBank { get; set; }
        public DbSet<TransformedEmployeeBank> transformedEmployeeBank { get; set; }
        public DbSet<EmployeeSalary> employeesalary { get; set; }
        public DbSet<HistoryEmployeeSalary> historyemployeesalary { get; set; }
        public DbSet<StagingEmployeeSalary> stagingEmployeeSalary { get; set; }
        public DbSet<TransformedEmployeeSalary> transformedEmployeeSalary { get; set; }
        public DbSet<EmployeePayDeduction> employeepaydeduction { get; set; }
        public DbSet<HistoryEmployeePayDeduction> historyemployeepaydeduction { get; set; }
        public DbSet<StagingEmployeePayDeduction> stagingEmployeePayDeductions { get; set; }
        public DbSet<TransformedEmployeePayDeduction> transformedEmployeePayDeductions { get; set; }
        public DbSet<Provider> provider { get; set; }
        public DbSet<GPRI> gpri { get; set; }
        public DbSet<GPRIFile> gprifile { get; set; }
        public DbSet<GPRIXML> gprixml { get; set; }
        public DbSet<LegalEntity> legalentity { get; set; }
        public DbSet<PayGroup> paygroup { get; set; }
        public DbSet<GenericList> lookupdetails { get; set; }
        public DbSet<PayCalendar> paycalendar { get; set; }
        public DbSet<Client> client { get; set; }
        public DbSet<EmployeeContrySpecific> employeecontryspecific { get; set; }
        public DbSet<HistoryEmployeeContrySpecific> historyemployeecontryspecific { get; set; }
        public DbSet<TimeAndAttendance> timeandattendance { get; set; }
        public DbSet<Country> country { get; set; }
        public DbSet<PayFrequency> payfrequency { get; set; }
        public DbSet<PayrollElements> payrollelements { get; set; }
        public DbSet<CountryPicklist> countrypicklist { get; set; }
        public DbSet<Errorlog> errorlog { get; set; }
        public DbSet<TaxAuthority> taxauthority { get; set; }
        public DbSet<EmployeeConf> employeeconf { get; set; }
        public DbSet<HistoryEmployeeConf> historyemployeeconf { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<UserPaygroupAssignment> userpaygroupassignment { get; set; }
        public DbSet<PeriodChangePERS> PERS { get; set; }
        public DbSet<PeriodChangeJDET> JDET { get; set; }
        public DbSet<PeriodChangeSLRY> SLRY { get; set; }
        public DbSet<PeriodChangeBANK> BANK { get; set; }
        public DbSet<PeriodChangeADDR> ADDR { get; set; }
        public DbSet<PeriodChangeCSPF> CSPF { get; set; }
        public DbSet<PeriodChangeCONF> CONF { get; set; }
        public DbSet<PeriodChangePAYD> PAYD { get; set; }
        public DbSet<PeriodChangeTIME> TIME { get; set; }
        public DbSet<PeriodChangeStarters> STARTERS { get; set; }
        public DbSet<PeriodChangeLeavers> Leavers { get; set; }
        public DbSet<TransactionResponseModel> TransactionResponseModel { get; set; }
        public DbSet<PaySlips> gpripayslip { get; set; }
        public DbSet<GPRIPayRunImports> gpripayrunimports { get; set; }
        public DbSet<PayPeriodRegistorDBValuesDetail> PayPeriodRegistorDBValuesDetail { get; set; }
        public DbSet<PayPeriodWithPrevioudCutOff> payperiodcutoffs { get; set; }
        public DbSet<ChangeLog> changelog { get; set; }
        public DbSet<BatchHistory> batchhistory { get; set; }
        public DbSet<countryspecificfields> countryspecificfields { get; set; }
        public DbSet<ResponseSelectListValueModel> responseSelectListValueModel { get; set; }
        public DbSet<ReportServiceDetails> reportservicedetails { get; set; }
    }
}