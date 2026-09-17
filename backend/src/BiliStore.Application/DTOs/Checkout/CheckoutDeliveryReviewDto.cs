namespace BiliStore.Application.DTOs.Checkout;

public sealed record CheckoutDeliveryReviewDto(
    string DeliveryMethod,
    string RecipientPhone,
    string PaxiStoreCode,
    string CollectionPointName,
    string TownCity,
    string Province,
    string? Instructions);
