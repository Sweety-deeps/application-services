using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ErrorLogRequestModel
    {
        public string? clientname {  get; set; }
        public string? paygroup {  get; set; }
        public string? filterby {  get; set; }
        public int? startpp {  get; set; }
        public int? endpp {  get; set; }
        public DateTime? startdate {  get; set; }
        public DateTime? enddate {  get; set; }

    }
    public class ErrorLogResponseModel
    {
        public int id { get; set; }
        public string? ClientName { get; set; }
        public DateTime? StartDate {  get; set; }
        public DateTime? EndDate { get; set; }
        public string? PayGroup { get; set; }
        public string EmployeeID { get; set; }
        public string? EventType { get; set; }
        public string? EntityState { get; set; }
        public string? Log { get; set; }
        public string? TableName { get; set; }
        public string Field { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? File { get; set; }
        public int? Line { get; set; }
        public string? ErrorRepotedOn { get; set; }
        public int RequestDetailsId { get; set; }

    }
}									
