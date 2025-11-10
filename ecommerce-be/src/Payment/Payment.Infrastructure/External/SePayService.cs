using Microsoft.Extensions.Configuration;
using Payment.Application.Abstractions.External;
using Payment.Application.Features.Dtos;
using System.Net.Http.Json;

namespace Payment.Infrastructure.External;

public class SePayService : ISePayApi
{
    private readonly IConfiguration _config;

    public SePayService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<SePayPaymentResponse> CreatePaymentAsync(SePayPaymentRequest req, CancellationToken ct)
    {
        var account = _config["SePay:AccountNo"];
        var bank = _config["SePay:BankName"];
        var amount = req.Amount;
        var des = Uri.EscapeDataString(req.Description ?? req.OrderCode);

        // Tạo link QR động theo chuẩn SePay
        var qrUrl = $"https://qr.sepay.vn/img?acc={account}&bank={bank}&amount={amount}&des={des}";

        var res = new SePayPaymentResponse
        {
            QrUrl = qrUrl,
            PaymentUrl = qrUrl, // dùng cùng giá trị
            OrderCode = req.OrderCode
        };

        return res;
    }
}