
namespace Domain.Models
{
    public class PaySlipsUploadFromS3Model
    {
        public string paygroup { get; set; }
        public string fileid { get; set; }
        public string s3objectid { get; set; }
    }
}
