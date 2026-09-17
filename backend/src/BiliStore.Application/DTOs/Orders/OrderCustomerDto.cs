namespace BiliStore.Application.DTOs.Orders;

public sealed record OrderCustomerDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone);
