
using Domain.Models;
using System.Data;
using System.Reflection;

namespace Services.Helpers
{
    public class ExportToExcelHelper
    {
        string base64String = string.Empty;

        #region Excel Reports
            
        ///// <summary>            
        ///// Generates Excel report for GPRI
        ///// </summary>
        ///// <param name="_gpri"></param>
        ///// <param name="_mdl"></param>
        ///// <returns></returns>
        //public string GetGPRIReportData(List<GPRI> _gpri, GPRIRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(_gpri);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.GPRIExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(1, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.payperiod : "";
        //            wb.Worksheet(1).Cell(5, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}

        ///// <summary>
        ///// Generates Excel report for PayPeriodRegister
        ///// </summary>
        ///// <param name="results"></param>
        ///// <param name="_mdl"></param>
        ///// <returns></returns>
        //public string GetPayPeriodRegisterReportData(List<PayPeriodRegisterResponseModel> results, PayPeriodRegisterRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.PayPeriodRegisterExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            wb.Worksheet(1).Cell(8, 2).Value = _mdl != null ? _mdl.startpp : "";
        //            wb.Worksheet(1).Cell(9, 2).Value = _mdl != null ? _mdl.endpp : "";
        //            wb.Worksheet(1).Cell(11, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}
        //public string GetHrDatawarehouseReportData(List<HrDatawarehouseResponseModel> results, HrDatawarehouseRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.hrdatawarehouseExcelTemplate)))
        //        {
        //            int nextrownumber = 0;
        //            var sheet = wb.Worksheet(1);
        //             wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.includeterminated : ""; 
        //            wb.Worksheet(1).Cell(10, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}

        //public string GetCSPFDatawarehouseReportData(List<CSPFDatawarehouseResponseModel> results, CSPFDatawarehouseRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.cspfdatawarehouseExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            // wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.includeterminated : "";
        //            wb.Worksheet(1).Cell(5, 2).Value = _mdl != null ? _mdl.startdate : "";
        //            wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.enddate : "";
        //            wb.Worksheet(1).Cell(10, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}

        //public string GetPAYDDatawarehouseReportData(List<PaydDatawarehouseResponseModel> results, PaydDatawarehouseRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.payddatawarehouseExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.includeterminated : "";
        //            wb.Worksheet(1).Cell(5, 2).Value = _mdl != null ? _mdl.payperiod : "";

        //            wb.Worksheet(1).Cell(12, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}

        //public string GetPeriodChangeFile(List<PeriodChangeFileResponseModel> results, PeriodChangeFileRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.periodchangefilereportExcelTemplate)))
        //        {
        //            int nextrownumber = 0;
        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            //wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.filteroption : "
        //            //wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.filteroption : "";
        //            wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            wb.Worksheet(1).Cell(5, 2).Value = _mdl != null ? _mdl.country : "";
        //            wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.report : "";
        //            //wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.filterby : "";
        //            //wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.ppoption : "";
        //            wb.Worksheet(1).Cell(7, 2).Value = _mdl != null ? _mdl.payperiod : "";
        //            wb.Worksheet(1).Cell(8, 2).Value = _mdl != null ? _mdl.startpp : "";
        //            wb.Worksheet(1).Cell(9, 2).Value = _mdl != null ? _mdl.endpp : "";
        //            wb.Worksheet(1).Cell(10, 2).Value = _mdl != null ? _mdl.payperiod : "";
        //            wb.Worksheet(1).Cell(11, 2).Value = _mdl != null ? _mdl.startdate : "";
        //            wb.Worksheet(1).Cell(12, 2).Value = _mdl != null ? _mdl.enddate : "";
        //            wb.Worksheet(1).Cell(15, 1).InsertTable(dt);

        //            //if (_mdl.filterby == "Pay Period")
        //            //{
        //            //    nextrownumber++;
        //            //    wb.Worksheet(1).Cell(nextrownumber, 2).Value = _mdl != null ? _mdl.filterby : "";
        //            //    nextrownumber++;
        //            //    wb.Worksheet(1).Cell(nextrownumber, 2).Value = _mdl != null ? _mdl.payperiod : "";

        //            //}
        //            //else
        //            //{
        //            //    nextrownumber++;
        //            //    wb.Worksheet(1).Cell(nextrownumber, 2).Value = _mdl != null ? _mdl.filterby : "";
        //            //    nextrownumber++;
        //            //    wb.Worksheet(1).Cell(nextrownumber, 2).Value = _mdl != null ? _mdl.startdate : "";
        //            //    nextrownumber++;
        //            //    wb.Worksheet(1).Cell(nextrownumber, 2).Value = _mdl != null ? _mdl.enddate : "";
        //            //}
        //            //wb.Worksheet(1).Cell(nextrownumber + 3, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}
        //public string GetPayElementReport(List<PayElementResponseModel> results, PayElementRequestModel _mdl, Stream responseStream)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);


        //        //var res = GetTemplateFromS3(Constants.payelementreportExcelTemplate).Result;
        //        //using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.payelementreportExcelTemplate)))
        //        //FileStream fileStream = responseStream as FileStream;

        //        //FileStream f1 = new FileStream("GPRIFile.xlsx", FileMode.Open, FileAccess.Read);
        //        //responseStream.Position = 0;

        //        using (MemoryStream memStream = new MemoryStream())
        //        {


        //            responseStream.CopyTo(memStream);
        //            memStream.Seek(0, SeekOrigin.Begin);
        //            memStream.Position = 0;
        //            //return memStream;


        //            using (var wb = new XLWorkbook(memStream))
        //            {
        //                var sheet = wb.Worksheet(1);
        //                wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //                wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.paygroup : "";

        //                wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.type : "";
        //                wb.Worksheet(1).Cell(6, 1).InsertTable(dt);

        //                using (var ms = new MemoryStream())
        //                {
        //                    wb.SaveAs(ms);
        //                    base64String = Convert.ToBase64String(ms.ToArray());
        //                }
        //            }

        //        }







        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}

        //public string GetSystemUserReport(List<SystemUserResponseModel> results, SystemUserRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.systemuserreportExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            wb.Worksheet(1).Cell(5, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}

        //public string GetTransactionReport(List<TransactionResponseModel> results, TransactionRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.transactionreportbypaygroupExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            if (_mdl.filteroption == "Pay Group")
        //            {
        //                wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.filteroption : "";
        //                wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            }
        //            else
        //            {
        //                wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.filteroption : "";
        //                wb.Worksheet(1).Cell(5, 2).Value = _mdl != null ? _mdl.country : "";
        //            }
        //            wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.filteroption : "";

        //            if (_mdl.ppoption == "Single")
        //            {
        //                wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.ppoption : "";
        //                wb.Worksheet(1).Cell(7, 2).Value = _mdl != null ? _mdl.payperiod : "";
        //            }
        //            else
        //            {
        //                wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.ppoption : "";
        //                wb.Worksheet(1).Cell(8, 2).Value = _mdl != null ? _mdl.startpp : "";
        //                wb.Worksheet(1).Cell(9, 2).Value = _mdl != null ? _mdl.endpp : "";
        //            }

        //            wb.Worksheet(1).Cell(11, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}
        //public string GetVarianceReport(List<VarianceResponseModel> results, VarianceRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.variancereportExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            //wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.type : "";
        //            //if (_mdl.filteroption == "Pay Group")
        //            //{
        //            //    wb.Worksheet(1).Cell(5, 2).Value = _mdl != null ? _mdl.filteroption : "";
        //            //    wb.Worksheet(1).Cell(7, 2).Value = _mdl != null ? _mdl.paygroups : "";
        //            //}
        //            //else
        //            //{
        //            //    wb.Worksheet(1).Cell(5, 2).Value = _mdl != null ? _mdl.filteroption : "";
        //            //    wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.payperiod : "";

        //            //}



        //            wb.Worksheet(1).Cell(10, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return base64String;
        //}
        //public string GetErrorLogReport(List<ErrorLogResponseModel> results, ErrorLogRequestModel _mdl)
        //{

        //    try
        //    {
        //        //var templateFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "ReportTemplate", "TestTemplate.xlsx"));

        //        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        //        DataTable dt = converter.ToDataTable(results);

        //        using (var wb = new XLWorkbook(GetExcelTemplatePath(Constants.errorlogreportExcelTemplate)))
        //        {

        //            var sheet = wb.Worksheet(1);
        //            wb.Worksheet(1).Cell(2, 2).Value = _mdl != null ? _mdl.clientname : "";
        //            wb.Worksheet(1).Cell(3, 2).Value = _mdl != null ? _mdl.paygroup : "";
        //            if (_mdl.filterby == "Pay Period")
        //            {
        //                wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.filterby : "";
        //                wb.Worksheet(1).Cell(7, 2).Value = _mdl != null ? _mdl.startpp : "";
        //                wb.Worksheet(1).Cell(8, 2).Value = _mdl != null ? _mdl.endpp : "";
        //            }
        //            else
        //            {
        //                wb.Worksheet(1).Cell(4, 2).Value = _mdl != null ? _mdl.filterby : "";
        //                wb.Worksheet(1).Cell(5, 2).Value = _mdl != null ? _mdl.startdate : "";
        //                wb.Worksheet(1).Cell(6, 2).Value = _mdl != null ? _mdl.enddate : "";

        //            }

        //            wb.Worksheet(1).Cell(11, 1).InsertTable(dt);

        //            using (var ms = new MemoryStream())
        //            {
        //                wb.SaveAs(ms);
        //                base64String = Convert.ToBase64String(ms.ToArray());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }
        //    return base64String;
        //}

        #endregion Excel Reports

        #region Common Methods

        public string GetExcelTemplatePath(string templateName)
        {
            string path = string.Empty;
            try
            {
                path = Path.Combine(Environment.CurrentDirectory, "ReportTemplate", templateName);
            }
            catch (Exception ex)
            {

            }
            return path;
        }



        public PeriodChangeFileDataModel ApplyPeriodChangeFilter(PeriodChangeFileRequestModel _req, PeriodChangeFileDataModel _reponse)
        {
            PeriodChangeFileDataModel res = new PeriodChangeFileDataModel();
            try
            {
                if (_reponse != null)
                {

                    if (_req.paygroup != null)
                    {
                        if (_reponse.personal.Count > 0)
                        {
                            _reponse.personal = _reponse.personal.Where(t => t.Paygroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.jobs.Count > 0)
                        {
                            _reponse.jobs = _reponse.jobs.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.salary.Count > 0)
                        {
                            _reponse.salary = _reponse.salary.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.address.Count > 0)
                        {
                            _reponse.address = _reponse.address.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.bank.Count > 0)
                        {
                            _reponse.bank = _reponse.bank.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.cspf.Count > 0)
                        {
                            _reponse.cspf = _reponse.cspf.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.payD.Count > 0)
                        {
                            _reponse.payD = _reponse.payD.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.time.Count > 0)
                        {
                            _reponse.time = _reponse.time.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.starters.Count > 0)
                        {
                            _reponse.starters = _reponse.starters.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                        if (_reponse.leavers.Count > 0)
                        {
                            _reponse.leavers = _reponse.leavers.Where(t => t.PayGroup?.Trim().ToUpper() == _req.paygroup.Trim().ToUpper()).ToList();
                        }
                    }
                    //if (_req.payperiod != null)
                    //{

                    //    //_reponse = _reponse.Where(t => t.PayPeriod == _req.payperiod).ToList();
                    //}
                    //if (_req.startpp != null)
                    //{
                    //    //_reponse = _reponse.Where(t => t.startpp == _req.startpp).ToList();
                    //}
                    //if (_req.endpp != null)
                    //{
                    //    //_reponse = _reponse.Where(t => t.endpp == _req.endpp).ToList();
                    //}
                    if (_req.startdate != null)
                    {
                        if (_reponse.personal.Count > 0)
                        {
                            //_reponse.personal = _reponse.personal.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.jobs.Count > 0)
                        {
                            //_reponse.jobs = _reponse.jobs.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.salary.Count > 0)
                        {
                            //_reponse.salary = _reponse.salary.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.address.Count > 0)
                        {
                            //_reponse.address = _reponse.address.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.bank.Count > 0)
                        {
                            //_reponse.bank = _reponse.bank.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.cspf.Count > 0)
                        {
                            //_reponse.cspf = _reponse.cspf.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.payD.Count > 0)
                        {
                            //_reponse.payD = _reponse.payD.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.time.Count > 0)
                        {
                            //_reponse.time = _reponse.time.Where(t => t.CreatedAt >= _req.startdate).ToList();
                        }
                        if (_reponse.starters.Count > 0)
                        {
                            //_reponse.starters = _reponse.starters.Where(t => t.HireDate >= _req.startdate).ToList();
                        }
                        if (_reponse.leavers.Count > 0)
                        {
                            //_reponse.leavers = _reponse.leavers.Where(t => t.TerminationDate >= _req.startdate).ToList();
                        }

                    }
                    if (_req.enddate != null)
                    {

                        if (_reponse.personal.Count > 0)
                        {
                            //_reponse.personal = _reponse.personal.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.jobs.Count > 0)
                        {
                            //_reponse.jobs = _reponse.jobs.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.salary.Count > 0)
                        {
                            //_reponse.salary = _reponse.salary.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.address.Count > 0)
                        {
                            // _reponse.address = _reponse.address.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.bank.Count > 0)
                        {
                            //_reponse.bank = _reponse.bank.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.cspf.Count > 0)
                        {
                            //_reponse.cspf = _reponse.cspf.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.payD.Count > 0)
                        {
                            //_reponse.payD = _reponse.payD.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.time.Count > 0)
                        {
                            //_reponse.time = _reponse.time.Where(t => t.CreatedAt <= _req.enddate).ToList();
                        }
                        if (_reponse.starters.Count > 0)
                        {
                            //_reponse.starters = _reponse.starters.Where(t => t.HireDate <= _req.enddate).ToList();
                        }
                        if (_reponse.leavers.Count > 0)
                        {
                            //_reponse.leavers = _reponse.leavers.Where(t => t.TerminationDate <= _req.enddate).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }

        public List<PayPeriodRegisterResponseModel> ApplyPayPeriodRegisterFilter(PayPeriodRegisterRequestModel _req, List<PayPeriodRegisterResponseModel> _reponse)
        {
            //List<PayPeriodRegisterResponseModel> res = new List<PayPeriodRegisterResponseModel>();
            try
            {
                if (_reponse != null)
                {

                    if (_req.paygroup != null)
                    {
                        _reponse = _reponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                    }

                    if (_req.startpp != null)
                    {

                    }
                    if (_req.endpp != null)
                    {

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }

        public List<PayElementResponseModel> ApplyPayElementFilter(PayElementRequestModel _req, List<PayElementResponseModel> _reponse)
        {
            //List<PayElementResponseModel> res = new List<PayElementResponseModel>();
            try
            {
                if (_reponse != null)
                {
                    if (!string.IsNullOrEmpty(_req.clientname))
                    {
                        _reponse = _reponse.Where(t => t.Clientname == _req.clientname).ToList();
                    }

                    if (!string.IsNullOrEmpty(_req.paygroup))
                    {
                        _reponse = _reponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }

        public List<SystemUserResponseModel> ApplySystemUserFilter(SystemUserRequestModel _req, List<SystemUserResponseModel> _reponse)
        {
            //List<SystemUserResponseModel> res = new List<SystemUserResponseModel>();
            try
            {
                if (_reponse != null)
                {
                    if (_req.clientname != null)
                    {
                        // _reponse = _reponse.Where(t => t.Clientname == _req.clientname).ToList();
                    }

                    if (_req.paygroup != null)
                    {
                        // _reponse = _reponse.Where(t => t.Paygroup == _req.paygroup).ToList();
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }

        public List<TransactionResponseModel> ApplyTransactionByPayGroupFilter(TransactionRequestModel _req, List<TransactionResponseModel> _reponse)
        {
            //List<TransactionResponseModel> res = new List<TransactionResponseModel>();
            try
            {
                if (_reponse != null)
                {

                    if (_req.clientname != null)
                    {
                        _reponse = _reponse.Where(t => t.ClientName == _req.clientname).ToList();
                    }
                    if (_req.paygroup != null)
                    {
                        _reponse = _reponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                    }
                    if (_req.startpp != null && _req.endpp != null)
                    {
                        _reponse = _reponse.Where(t => Convert.ToInt32(t.PayPeriod) >= Convert.ToInt32(_req.startpp) && Convert.ToInt32(t.PayPeriod) <= Convert.ToInt32(_req.endpp)).ToList();
                    }
                    else if (_req.startpp == null && _req.endpp != null)
                    {
                        _reponse = _reponse.Where(t => Convert.ToInt32(t.PayPeriod) <= Convert.ToInt32(_req.endpp)).ToList();
                    }
                    else if (_req.startpp == null && _req.endpp != null)
                    {
                        _reponse = _reponse.Where(t => Convert.ToInt32(t.PayPeriod) >= Convert.ToInt32(_req.startpp)).ToList();
                    }



                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }
        public List<TransactionCountryResponseModel> ApplyTransactionByCountryFilter(TransactionCountryRequestModel _req, List<TransactionCountryResponseModel> _reponse)
        {
            //List<TransactionResponseModel> res = new List<TransactionResponseModel>();
            try
            {
                if (_reponse != null)
                {

                    if (_req.clientname != null)
                    {
                        _reponse = _reponse.Where(t => t.ClientName == _req.clientname).ToList();
                    }
                    if (_req.country != null)
                    {
                        _reponse = _reponse.Where(t => t.Country == _req.country).ToList();
                    }
                    if (_req.startpp != null && _req.endpp != null)
                    {
                        _reponse = _reponse.Where(t => Convert.ToInt32(t.PayPeriod) >= Convert.ToInt32(_req.startpp) && Convert.ToInt32(t.PayPeriod) <= Convert.ToInt32(_req.endpp)).ToList();
                    }
                    else if (_req.startpp == null && _req.endpp != null)
                    {
                        _reponse = _reponse.Where(t => Convert.ToInt32(t.PayPeriod) <= Convert.ToInt32(_req.endpp)).ToList();
                    }
                    else if (_req.startpp == null && _req.endpp != null)
                    {
                        _reponse = _reponse.Where(t => Convert.ToInt32(t.PayPeriod) >= Convert.ToInt32(_req.startpp)).ToList();
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }
        public List<ErrorLogResponseModel> ApplyErrorLogFilter(ErrorLogRequestModel _req, List<ErrorLogResponseModel> _reponse)
        {
            //List<ErrorLogResponseModel> res = new List<ErrorLogResponseModel>();
            if (_reponse != null)
            {
                if (_req.paygroup != null)
                {
                    _reponse = _reponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                }
                if (_req.startpp != null)
                {
                    //_reponse = _reponse.Where(t => t.StartPP == _req.startpp).ToList();
                }
                if (_req.endpp != null)
                {
                    //_reponse = _reponse.Where(t => t.EndPP == _req.endpp).ToList();
                }
                if (_req.startdate != null)
                {
                    //_reponse = _reponse.Where(t => t.StartDate == _req.startdate).ToList();
                }
                if (_req.enddate != null)
                {
                    //_reponse = _reponse.Where(t => t.EndDate == _req.enddate).ToList();

                }
            }


            return _reponse;
        }


        public List<HrDatawarehouseResponseModel> ApplyHRDataWarehouseFilter(HrDatawarehouseRequestModel _req, List<HrDatawarehouseResponseModel> _reponse, DateTime _payPeriodEndDate)
        {
            try
            {
                if (_reponse != null)
                {
                    if (_req.year != null && _req.year != 0)
                    {
                        _reponse = _reponse.Where(t => t.year == _req.year).ToList();
                    }

                    if (_req.payperiod != null)
                    {
                        if (_req.includeterminated != null)
                        {
                            if (_req.includeterminated.ToLower() == "yes")
                            {
                                _reponse = _reponse.Where(t => Convert.ToDateTime(t.HireDate) <= _payPeriodEndDate).ToList();
                            }
                            else if (_req.includeterminated.ToLower() == "no")
                            {
                                _reponse = _reponse.Where(t => Convert.ToDateTime(t.HireDate) <= _payPeriodEndDate && (Convert.ToDateTime(t.TerminationDate) >= DateTime.UtcNow || t.TerminationDate == null)).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (_req.includeterminated != null)
                        {
                            if (_req.includeterminated.ToLower() == "no")
                            {
                                _reponse = _reponse.Where(t => Convert.ToDateTime(t.TerminationDate) >= DateTime.UtcNow || t.TerminationDate == null).ToList();
                            }
                        }

                    }

                }

            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }

        public CSAndCFDatawarehouseResponseModel ApplyCSPFDataWarehouseFilter(CSPFDatawarehouseRequestModel _req, CSAndCFDatawarehouseResponseModel _reponse)
        {
            try
            {
                if (_reponse.cspfDatawarehouseResponse != null)
                {
                    if (_req.clientname != null)
                    {
                        //_reponse = _reponse.Where(t => t.ClientName == _req.clientname).ToList();
                    }
                    if (_req.paygroup != null)
                    {
                        _reponse.cspfDatawarehouseResponse = _reponse.cspfDatawarehouseResponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                    }
                    if (_req.includeterminated != null)
                    {
                        //_reponse = _reponse.Where(t => t.IncludeTermination == _req.includedeterination).ToList();
                    }
                    if (_req.startdate != null)
                    {
                        //_reponse = _reponse.Where(t => t.StartDate == _req.startdate).ToList();
                    }
                    if (_req.enddate != null)
                    {
                        //  _reponse = _reponse.Where(t => t.EndDate == _req.enddate).ToList();
                    }
                }
                if (_reponse.confidentialDatawarehouseResponse != null)
                {
                    if (_req.paygroup != null)
                    {
                        _reponse.confidentialDatawarehouseResponse = _reponse.confidentialDatawarehouseResponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }
        public List<VarianceResponseModel> ApplyVarianceFilter(List<VarianceResponseModel> _old, List<VarianceResponseModel> _new)
        {
            List<VarianceResponseModel> res = new List<VarianceResponseModel>();

            try
            {

                foreach (var itemN in _new)
                {

                    var matchedOldItems = _old.Where(itemO =>
                        itemN.EmpolyeeID == itemO.EmpolyeeID &&
                        itemN.PayElementCode == itemO.PayElementCode &&
                        itemN.PayGroup == itemO.PayGroup);
                    if (matchedOldItems.Any())
                    {
                        foreach (var matchedItem in matchedOldItems)
                        {

                            VarianceResponseModel temp = new VarianceResponseModel
                            {
                                PayGroup = itemN.PayGroup,
                                LastName = itemN.LastName,
                                MiddleName = itemN.MiddleName,
                                FirstName = itemN.FirstName,
                                SecondLastName = itemN.SecondLastName,
                                EmpolyeeID = itemN.EmpolyeeID,
                                PayElementNameLocal = itemN.PayElementNameLocal,
                                PayElementName = itemN.PayElementName,
                                PayElementCode = itemN.PayElementCode,
                                ExportCode = itemN.ExportCode,
                                NewAmount = itemN.NewAmount,
                                OldAmount = matchedItem.NewAmount,
                                VarianceAmount = itemN.NewAmount - matchedItem.NewAmount,
                                VariancePercentage =Math.Round(matchedItem.NewAmount != 0 ? ((itemN.NewAmount - matchedItem.NewAmount) / matchedItem.NewAmount) * 100 : 0,2),
                                NewPeriod = itemN.NewPeriod,
                                OldPeriod = matchedItem.NewPeriod
                            };
                            res.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }
            return res;
        }
        public List<CalendarResponseModel> ApplyCalendarReportFilter(CalendarRequestModel _req, List<CalendarResponseModel> _reponse)
        {
            List<CalendarResponseModel> res = new List<CalendarResponseModel>();
            try
            {
                if (_reponse != null)
                {
                    if (_req.clientname != null)
                    {
                        //_reponse = _reponse.Where(t => t.ClientName == _req.clientname).ToList();
                    }
                    if (_req.paygroup != null)
                    {
                        //_reponse = _reponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                    }
                    if (_req.year != null)
                    {
                        //_reponse = _reponse.Where(t => t.Type == _req.type).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }
        public List<DeltaResponseModel> ApplyDeltaReport(DeltaRequestModel _req, List<DeltaResponseModel> _reponse)
        {
            List<DeltaResponseModel> res = new List<DeltaResponseModel>();
            try
            {
                if (_reponse != null)
                {
                    //if (_req.clientname != null)
                    //{
                    //    //_reponse = _reponse.Where(t => t.ClientName == _req.clientname).ToList();
                    //}
                    //if (_req.paygroup != null)
                    //{
                    //    //_reponse = _reponse.Where(t => t.PayGroup == _req.paygroup).ToList();
                    //}
                    //if (_req.year != null)
                    //{
                    //    //_reponse = _reponse.Where(t => t.Type == _req.type).ToList();
                    //}
                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }

        public List<PaydDatawarehouseResponseModel> ApplyPaydDataWarehouseFilter(PaydDatawarehouseRequestModel _req, List<PaydDatawarehouseResponseModel> _reponse, DateTime _payPeriodEndDate)
        {
            try
            {
                if (_reponse != null)
                {
                    if (_req.payperiod != null)
                    {
                        if (_req.includeterminated != null)
                        {
                            if (_req.includeterminated.ToLower() == "yes")
                            {
                                _reponse = _reponse.Where(t => Convert.ToDateTime(t.EffectiveDate) <= _payPeriodEndDate).ToList();
                            }
                            else if (_req.includeterminated.ToLower() == "no")
                            {
                                _reponse = _reponse.Where(t => Convert.ToDateTime(t.EffectiveDate) <= _payPeriodEndDate && (Convert.ToDateTime(t.EndDate) >= DateTime.UtcNow || t.EndDate == null)).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (_req.includeterminated != null)
                        {
                            if (_req.includeterminated.ToLower() == "no")
                            {
                                _reponse = _reponse.Where(t => Convert.ToDateTime(t.EndDate) >= DateTime.UtcNow || t.EndDate == null).ToList();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return _reponse;
        }

        #endregion Common Methods



    }


    public class ListtoDataTableConverter
    {
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}



