namespace BiliStore.Domain.Enums;

public enum OrderStatus
{
    PendingPayment = 0,
    Paid = 1,
    MeasurementsConfirmed = 2,
    InProduction = 3,
    ReadyForDelivery = 4,
    SentViaPaxi = 5,
    ReadyForCollection = 6,
    Completed = 7,
    Cancelled = 8
}

