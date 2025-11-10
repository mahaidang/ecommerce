namespace Payment.Application.Features.Dtos;

public class SePayWebhookDto
{
    public long Id { get; set; }
    public string? Gateway { get; set; }
    public string? TransactionDate { get; set; }
    public string? AccountNumber { get; set; }
    public string? Code { get; set; }              // mã định danh nếu có
    public string? Content { get; set; }           // nội dung chuyển khoản
    public string? TransferType { get; set; }      // "in" hoặc "out"
    public decimal TransferAmount { get; set; }    // số tiền giao dịch
    public decimal Accumulated { get; set; }       // số dư sau giao dịch
    public string? SubAccount { get; set; }
    public string? ReferenceCode { get; set; }     // mã tham chiếu
    public string? Description { get; set; }       // nội dung tin nhắn sms
}