using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PayloadRequestDetails
    {
        public int? PayloadRequestDetailId { get; set; }
        public int? OrganisationID { get; set; }
        public int? PayroleCompanyID { get; set; }
        public int? PayGroupID { get; set; }
        public int? PayloadTypeID { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public byte[] MsgData { get; set; }
    }
}
