namespace BiliStore.Application.DTOs.Customers;

public sealed record CreateReturnRequestDto(
    string Reason,
    string Details);
