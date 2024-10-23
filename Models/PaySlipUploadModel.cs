using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class PaySlipsUploadModel
    {
        public string paygroup { get; set; }
        public string fileid { get; set; }
        public string s3objectid { get; set; }
        public long filesize { get; set; }
    }
}
