using Domain.Models;
using Domain.Models.Payslips;

namespace Services.Dayforce
{
    public interface IDayforceApiClient
	{
        Task<BaseResponseModel<string>> PostGpriAsync(DayforceGpriRequestModel requestModel, PaygroupBaseGpriModel paygroupBaseGpriModel);
        Task<BaseResponseModel<PostPayslipResult>> PostPayslipAsync(DayforcePostPayslipListRequest requestModel);
    }
}
