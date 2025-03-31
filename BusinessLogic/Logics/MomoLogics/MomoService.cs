using System.Security.Cryptography;
using System.Text;
using Client.Logics.Commons.MomoLogics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace BusinessLogic.Logics.MomoLogics;

public class MomoService : IMomoService
{
    private readonly IOptions<MomoOptionModel> _options;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options"></param>
    public MomoService(IOptions<MomoOptionModel> options)
    {
        _options = options;
    }

    public async Task<MomoCreatePaymentResponseModel> CreatePaymentOrderAsync(MomoExecuteResponseModel model)
    {
        // Add extra data to the request
        var extraData = $"{model.FullName}|";

        // Create the raw data for the request
        var rawData =
            $"partnerCode={_options.Value.PartnerCode}" +
            $"&accessKey={_options.Value.AccessKey}" +
            $"&requestId={model.OrderId}" +
            $"&amount={model.Amount}" +
            $"&orderId={model.OrderId}" +
            $"&orderInfo={model.OrderInfo}" +
            $"&returnUrl={_options.Value.ReturnUrlOrder}" +
            $"&notifyUrl={_options.Value.NotifyUrl}" +
            $"&extraData={extraData}";

        // Compute the signature
        var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

        // Create the request
        var client = new RestClient(_options.Value.MomoApiUrl);
        var request = new RestRequest() { Method = Method.Post };
        request.AddHeader("Content-Type", "application/json; charset=UTF-8");

        // Create an object representing the request data to Momo
        var requestData = new
        {
            accessKey = _options.Value.AccessKey,
            partnerCode = _options.Value.PartnerCode,
            notifyUrl = _options.Value.NotifyUrl,
            returnUrl = _options.Value.ReturnUrlOrder,
            requestType = _options.Value.RequestType,
            orderId = model.OrderId,
            amount = model.Amount.ToString(),
            orderInfo = model.OrderInfo,
            requestId = model.OrderId,
            extraData = extraData,
            signature = signature
        };

        // Add the request data to the request
        request.AddParameter("application/json", JsonConvert.SerializeObject(requestData),
            ParameterType.RequestBody);

        // Execute the request
        var response = await client.ExecuteAsync(request);
        var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        Console.WriteLine("Raw Momo Response: " + response.Content);

        return momoResponse;
    }

    /// <summary>
    /// Parse the response from Momo after the payment is executed
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public MomoExecuteResponseModel PaymentExecuteOrderAsync(IQueryCollection collection)
    {
        var errorCode = collection.First(s => s.Key == "errorCode").Value;
        var amount = collection.First(s => s.Key == "amount").Value;
        var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
        var orderId = collection.First(s => s.Key == "orderId").Value;
        var transId = collection.First(s => s.Key == "transId").Value;
        var extraData = collection.First(s => s.Key == "extraData").Value;
        var res = extraData.ToString().Split('|');

        return new MomoExecuteResponseModel()
        {
            ErrorCode = errorCode,
            FullName = res[0],
            Amount = amount,
            OrderId = orderId,
            OrderInfo = orderInfo,
            TransactionId = transId,
            ExtraData = extraData,
        };
    }

    /// <summary>
    /// Compute the HMAC SHA256 signature for the request
    /// </summary>
    /// <param name="message"></param>
    /// <param name="secretKey"></param>
    /// <returns></returns>
    public string ComputeHmacSha256(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        byte[] hashBytes;

        using (var hmac = new HMACSHA256(keyBytes))
        {
            hashBytes = hmac.ComputeHash(messageBytes);
        }

        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hashString;
    }
}