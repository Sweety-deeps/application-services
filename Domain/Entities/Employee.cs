namespace Domain.Entities
{
    public class Employee
    {
        public int id { get; set; }
        public string employeeid { get; set; }
        public string? employeenumber { get; set; }
        public int paygroupid { get; set; }
        public string paygroup { get; set; }
        public DateTime? hiredate { get; set; }
        public string? lastname { get; set; }
        public string? secondlastname { get; set; }
        public string? firstname { get; set; }
        public string? middlenames { get; set; }
        public string? title { get; set; }
        public string? gender { get; set; }
        public DateTime? dateofbirth { get; set; }
        public string? birthcity { get; set; }
        public string? birthcountry { get; set; }
        public DateTime? dateofleaving { get; set; }
        public string? terminationreason { get; set; }
        public DateTime? senioritydate { get; set; }
        public string? maritalstatus { get; set; }
        public string? workemail { get; set; }
        public string? personalemail { get; set; }
        public string? workphone { get; set; }
        public string? mobilephone { get; set; }
        public string? homephone { get; set; }
        public string createdby { get; set; }
        public DateTime? createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public string? nationality { get; set; }
        public string? comments { get; set; }
        public DateTime? effectivedate { get; set; }
        public DateTime? companystartdate { get; set; }
    }
}
