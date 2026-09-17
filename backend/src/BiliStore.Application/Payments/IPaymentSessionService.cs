using BiliStore.Application.DTOs.Payments;

namespace BiliStore.Application.Payments;

public interface IPaymentSessionService
{
    Task<CreatePaymentSessionResponseDto> CreateSessionAsync(
        CreatePaymentSessionRequestDto request,
        CancellationToken cancellationToken);
}
