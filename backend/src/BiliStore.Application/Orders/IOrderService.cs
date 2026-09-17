using BiliStore.Application.DTOs.Orders;

namespace BiliStore.Application.Orders;

public interface IOrderService
{
    Task<CreateOrderResponseDto> CreateAsync(
        CreateOrderRequestDto request,
        Guid? applicationUserId,
        CancellationToken cancellationToken);

    Task<OrderConfirmationDto?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken);
}
