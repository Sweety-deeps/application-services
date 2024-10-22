using Domain.Models;
using Domain.Models.Payslips;

namespace Services.Dayforce
{
    public interface IDayforceSftpClient
    {
        Task<BaseResponseModel<string>> PostGpriSftpAsync(DayforceGpriRequestModel requestModel);
        Task<BaseResponseModel<PostPayslipResult>> PostPayslipSftpAsync(DayforcePostPayslipListRequest requestModel);
    }
}
