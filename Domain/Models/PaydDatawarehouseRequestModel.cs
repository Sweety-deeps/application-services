namespace Domain.Models
{
    public class PaydDatawarehouseRequestModel
    {
        public string? clientname { get; set; }
        public string? paygroup { get; set; }
        public string? includeterminated { get; set; }
        public string? payperiod { get; set; }
        public int? year { get; set; }
    }

    public class PaydDatawarehouseResponseModel
    {
        public string? clientname { get; set; }
        public string? paygroup { get; set; }
        public string? includeterminated { get; set; }
        public string? payperiod { get; set; }
        public string? PayGroup { get; set; }
        public string? EmployeeID { get; set; }
        public string? EffectiveDate { get; set; }
        public string? EndDate { get; set; }
        public string? PayElementType { get; set; }
        public string? PayElementName { get; set; }
        public string? PayElementCode { get; set; }
        public string? ExportCode {  get; set; }
        public string? RecurrentSchedule { get; set; }
        public float? Amount { get; set; }
        public float? Percentage { get; set; }
        public string? PayDate { get; set; }
        public string? BussinessDate { get; set; } 
        public string? PayPeriodNumber { get; set; }
        public string? PayPeriodNumberSuffix { get; set; }
        public string? Message { get; set;} 
        public string? CostCenter { get; set; } 
        public string? PayrollFlag { get; set; }
        public string? ModificationOwner { get; set; }
        public string? ModificationDate { get; set; }

        public string? EmployeeStatus { get; set; }
    }
}
