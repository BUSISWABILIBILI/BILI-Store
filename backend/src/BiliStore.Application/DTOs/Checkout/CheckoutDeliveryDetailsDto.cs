namespace BiliStore.Application.DTOs.Checkout;

public sealed record CheckoutDeliveryDetailsDto(
    string RecipientPhone,
    string PaxiStoreCode,
    string CollectionPointName,
    string TownCity,
    string Province,
    string? Instructions);
