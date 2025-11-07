using Microsoft.Extensions.Configuration;
using Payment.Application.Abstractions.External;
using Payment.Application.Features.Dtos;
using System.Net.Http.Json;

namespace Payment.Infrastructure.External;

public class SePayService : ISePayApi
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public SePayService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<SePayPaymentResponse> CreatePaymentAsync(SePayPaymentRequest req, CancellationToken ct)
    {
        var url = _config["SePay:BaseUrl"] + "/api/v1/payment/create";
        var token = _config["SePay:ApiKey"];

        _http.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await _http.PostAsJsonAsync(url, req, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SePayPaymentResponse>(ct)
               ?? throw new InvalidOperationException("Invalid response from SePay");
    }
}