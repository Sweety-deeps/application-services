using Domain.Enums;

namespace Domain.Models
{
    public class DeltaResponseModel
    {
        public string? Paygroup { get; set; }
        public string? EmployeeID { get; set; }
        public string? TableName { get; set; }
        public string? FieldName { get; set; }
        public string? RecordType { get; set; }
        public string? OldValue { get; set; }
        public DateTime? OldEffectiveDate { get; set; }
        public string? NewValue { get; set; }
        public DateTime? NewEffectiveDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ChangeEffectiveType Type { get; set; }

        public int GetTablePriority(string tableName)
        {
            switch (tableName)
            {
                case "Personal":
                    return 1;
                case "Address":
                    return 2;
                case "Bank":
                    return 3;
                case "Conf":
                    return 4;
                case "Country Specific":
                    return 5;
                case "Job":
                    return 6;
                case "Pay Deduction":
                    return 7;
                case "Salary":
                    return 8;
                default:
                    return 9; // Default priority for unknown table names
            }
        }
    }


}
