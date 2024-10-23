using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StagingEmployee
    {
        public int id { get; set; }
        public int requestid { get; set; }
        public string? eventtype { get; set; }
        public string? entitystate { get; set; }
        public string? employeeid { get; set; }
        public string? employeenumber { get; set; }
        public string? paygroup { get; set; }
        public string? hiredate { get; set; }
        public string? lastname { get; set; }
        public string? secondlastname { get; set; }
        public string? firstname { get; set; }
        public string? middlenames { get; set; }
        public string? title { get; set; }
        public string? gender { get; set; }
        public string? dateofbirth { get; set; }
        public string? birthcity { get; set; }
        public string? birthcountry { get; set; }
        public string? dateofleaving { get; set; }
        public string? terminationreason { get; set; }
        public string? senioritydate { get; set; }
        public string? maritalstatus { get; set; }
        public string? contacttype { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? workemail { get; set; }
        public string? personalemail { get; set; }
        public string? workphone { get; set; }
        public string? mobilephone { get; set; }
        public string? homephone { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
        public string? modifiedby { get; set; }
        public DateTime? modifiedat { get; set; }
        public string? status { get; set; }
    }
}
