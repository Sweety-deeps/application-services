namespace Domain.Entities
{

    public class StagingEmployeeConf
        {
            public int id { get; set; }
            public int requestid { get; set; }
            public string? entitystate { get; set; }
            public string? employeeid { get; set; }
            public string? paygroup { get; set; }
            public string? effectivedate { get; set; }
            public string? enddate { get; set; }
            public string? documenttype { get; set; }
            public string? documentnumber { get; set; }
            public string? country { get; set; }
            public string createdby { get; set; }
            public DateTime createdat { get; set; }
            public string? modifiedby { get; set; }
            public DateTime? modifiedat { get; set; }
            public string? status { get; set; }
        }

        public class TransformedEmployeeConf
        {
            public int id { get; set; }
            public int requestid { get; set; }
            public string? entitystate { get; set; }
            public string? employeeid { get; set; }
            public string? paygroup { get; set; }
            public string? effectivedate { get; set; }
            public string? enddate { get; set; }
            public string? documenttype { get; set; }
            public string? documentnumber { get; set; }
            public string? country { get; set; }
            public string createdby { get; set; }
            public DateTime createdat { get; set; }
            public string? modifiedby { get; set; }
            public DateTime? modifiedat { get; set; }
            public string? errordetails { get; set; }
            public string? status { get; set; }
        }

    public class EmployeeConf
    {
        public int id { get; set; }
        public string employeeid { get; set; }
        public int paygroupid { get; set; }
        public string paygroup { get; set; }
        public DateTime? effectivedate { get; set; }
        public DateTime? enddate { get; set; }
        public string? documenttype { get; set; }
        public string? documentnumber { get; set; }
        public string? country { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
        public DateTime? expirydate { get; set; }
        public DateTime? issuedate { get; set; }
        public string? placeofissue { get; set; }

    }

    public class HistoryEmployeeConf
        {
            public int id { get; set; }
            public int conftableid { get; set; }
            public string entitystate { get; set; }
            public string? employeeid { get; set; }
            public int? paygroupid { get; set; }
            public DateTime? effectivedate { get; set; }
            public DateTime? enddate { get; set; }
            public string? documenttype { get; set; }
            public string? documentnumber { get; set; }
            public string? country { get; set; }
            public string createdby { get; set; }
            public DateTime createdat { get; set; }
            public string? modifiedby { get; set; }
            public DateTime? modifiedat { get; set; }
            public string? status { get; set; }
            public DateTime? expirydate { get; set; }
            public DateTime? issuedate { get; set; }
            public string? placeofissue { get; set; }
            public int? requestid { get; set; }

    }
}

