namespace Domain
{
    public class Constants
    {
        public const string ENVIRONMENT_VARIABLES_SECRET_KEY = "CONFIG_SECRET_MANAGER";
        public const string ENVIRONMENT_VARIABLES_AWS_REGION = "AWSREGION";
        public const string DB_CONNECTION_STRING = "DBConnectionString";
        public const string ENVIRONMENT_VARIABLES_PUBLISHERSQS = "PublisherSQS";
        public const string UNICODENCRYPTDECRYPTKEY = "NBJ42RKQ2vQoYFZO";

        #region S3

        public const string PAYSLIPBUCKETNAME = "ultipay-2.0-payslips";
        public const string PAYSLIPENVPATH = "dev/";
        public const string INBOUND_JSON_BUCKETNAME = "InboundJson_S3BucketName";
        public const string DATAIMPORT_BUCKETNAME = "DataImport_S3BucketName";
        public const string ERROR_DETAILS_BUCKETNAME =  "S3ErrorDetailsBucketName";

        #endregion S3




        #region GPRI/PaySlip Status

        public const string PAYROLLPROCESSING = "Payroll Results - Processing";
        public const string PAYROLLFAILED = "Payroll Results - Failed";
        public const string PAYROLLUPLOADED = "Payroll Results - Uploaded";
        public const string PAYROLLAPPROVED = "Payroll Results - Approved";
        public const string PAYROLLREJECTED = "Payroll Results - Rejected";
        public const string GPRIPROCESSING = "GPRI - Processing";
        public const string GPRIFAILED = "GPRI - Failed";
        public const string GPRICOMPLETED = "GPRI - Complete";
        public const string GPRIAPPROVED = "Approved";
        public const string GPRIREJECTED = "Rejected";

        public const string PAYROLLPROCESSINGNEXT = "";
        public const string PAYROLLFAILEDNEXT = "Review failure reason";
        public const string PAYROLLUPLOADEDNEXT = "Download Pay Period Register data";
        public const string PAYROLLAPPROVEDNEXT = "Send GPRI to Client";
        public const string GPRIPROCESSINGNEXT = "";
        public const string GPRIFAILEDNEXT = "Review failure reason, fix and reupload Payroll Register file";
        public const string GPRICOMPLETEDNEXT = "Client to review and approve Payroll";
        public const string GPRIAPPROVEDNEXT = "Proceed to process Payslip (if applicable)";
        public const string REJECTEDNEXT = "None.";

        public const string UPLOADQUEUED = "Queued";
        public const string UPLOADPROCESSING = "Upload - Processing";
        public const string UPLOADFAILED = "Upload - Failed";
        public const string UPLOADCOMPLETE = "Upload - Complete";
        public const string PAYSLIPSPROCESSING = "Payslips - Processing";
        public const string PAYSLIPSPARTIAL = "Payslips - Partially Complete";
        public const string PAYSLIPSFAILED = "Payslips - Failed";
        public const string PAYSLIPSCOMPLETE = "Payslips - Complete";

        public const string UPLOADPROCESSINGNEXT = "";
        public const string UPLOADFAILEDNEXT = "Review failure reason, fix and reupload Payslip";
        public const string UPLOADCOMPLETENEXT = "Download Payslip Matched data";
        public const string PAYSLIPSPROCESSINGNEXT = "";
        public const string PAYSLIPSPARTIALNEXT = "Review failure reason";
        public const string PAYSLIPSFAILEDNEXT = "Review failure reason";
        public const string PAYSLIPSCOMPLETENEXT = "";

        public const string POSTXML = "XML";
        public const string POSTJSON = "JSON";

        public const string CONNECTEDPAYPAYSLIP = "ConnectedPayPayslip";
        #endregion

        #region DataImport/ImportType

        public const string FixedList = "FixedList";

        #endregion
    }
}
