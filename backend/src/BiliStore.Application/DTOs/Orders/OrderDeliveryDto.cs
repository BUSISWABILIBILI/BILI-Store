namespace BiliStore.Application.DTOs.Orders;

public sealed record OrderDeliveryDto(
    string DeliveryMethod,
    string RecipientPhone,
    string PaxiStoreCode,
    string CollectionPointName,
    string TownCity,
    string Province,
    string? Instructions,
    string DeliveryStatus);
