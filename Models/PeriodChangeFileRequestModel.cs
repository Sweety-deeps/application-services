using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class PcFileRequestModel
    {
        [Required]
        public string PaygroupCode { get; set; }
        [Required]
        public string FilterOption { get; set; }
        public int? Year { get; set; }
        public int? PayPeriod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class PayPeriodWithPrevioudCutOff
    {
        [Column("c_id")]
        public int CurrentId { get; set; }
        [Column("c_payperiod")]
        public int CurrentPayperiod { get; set; }
        [Column("c_cutoffdate")]
        public DateTime? CurrentCutOffDate { get; set; }
        [Column("p_id")]
        public int? PreviousId { get; set; }
        [Column("p_payperiod")]
        public int? PreviousPayPeriod { get; set; }
        [Column("p_cutoffdate")]
        public DateTime? PreviousCutOffDate { get; set; }
    }

    public class PeriodChangeFileRequestModel
    {
        public string? clientname {  get; set; }
        //public string? filteroption{  get; set; }
        public string? country { get; set; }
        public string? paygroup { get; set; }
        public int? paygroupid { get; set; }
        //public string? ppoption { get; set; }
        public string? report { get; set; }
        public float? payperiod { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public int? year {  get; set; }
        public string? startpp {  get; set; }
        public string? single {  get; set; }
        public string? multiple {  get; set; }
        public string? endpp {  get; set; }            
    }

    public class PeriodChangeFileDataModel
    {
        public List<PeriodChangePERS> personal { get; set; }
        public List<PeriodChangeJDET> jobs { get; set; }
        public List<PeriodChangeSLRY> salary { get; set; }
        public List<PeriodChangeADDR> address { get; set; }
        public List<PeriodChangeBANK> bank { get; set; }
        public List<PeriodChangeCSPF> cspf { get; set; }
        public List<PeriodChangePAYD> payD { get; set; }
        public List<PeriodChangeCONF> conf { get; set; }
        public List<PeriodChangeTIME> time { get; set; } 
        public List<PeriodChangeStarters> starters { get; set; }
        public List<PeriodChangeLeavers> leavers { get; set; }

        public string PayGroup { get; set; }
        public string EmployeeID { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? HireDate { get; set; }
        public string? TerminationDate { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string? Title { get; set; }
        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? BirthCity { get; set; }
        public string? BirthCountry { get; set; }
        public string? Nationality { get; set; }
        public string? TerminationReason { get; set; }
        public string? SeniorityDate { get; set; }
        public string? WorkEmail { get; set; }
        public string? PersonalEmail { get; set; }
        public string? WorkPhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? HomePhone { get; set; }


        // filters
        public string? country { get; set; }
        public string? clientname { get; set; }
        public float? PayPeriod { get; set; }
        public string? startpp { get; set; }
        public string? endpp { get; set; }
    }
}
