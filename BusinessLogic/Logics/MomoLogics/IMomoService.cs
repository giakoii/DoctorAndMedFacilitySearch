using Client.Logics.Commons.MomoLogics;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Logics.MomoLogics
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentOrderAsync(MomoExecuteResponseModel model);

        MomoExecuteResponseModel PaymentExecuteOrderAsync(IQueryCollection collection);

        string ComputeHmacSha256(string message, string secretKey);
    }
}
