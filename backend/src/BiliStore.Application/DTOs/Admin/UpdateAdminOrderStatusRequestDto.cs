namespace BiliStore.Application.DTOs.Admin;

public sealed record UpdateAdminOrderStatusRequestDto(
    string OrderStatus,
    string? PaymentStatus);
