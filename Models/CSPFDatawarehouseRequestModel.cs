using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CSPFDatawarehouseRequestModel
    {
        public string? clientname { get; set; }
        public string? paygroup { get; set; }
        public string? includeterminated { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public string? EmployeeStatus {  get; set; }
    }

    public class CFDatawarehouseResponseModel
    {
        public string PayGroup { get; set; }
        public string EmployeeID { get; set; } 
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Country { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? PlaceOfIssue { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
    public class CSPFDatawarehouseResponseModel
    {
        public string PayGroup { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClientName { get; set; }
        public string? CsFIdName { get; set; }
        public string? Value { get; set; }
        public DateTime? StartDate { get; set; } 
        public string? includeterminated { get; set; }
        public string? EmployeeStatus { get; set; }
    }

    public class CSAndCFDatawarehouseResponseModel
    {
       public List<CSPFDatawarehouseResponseModel> cspfDatawarehouseResponse {  get; set; }
       public List<CFDatawarehouseResponseModel> confidentialDatawarehouseResponse { get; set; }
    }
}
