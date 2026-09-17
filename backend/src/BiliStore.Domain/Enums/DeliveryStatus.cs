namespace BiliStore.Domain.Enums;

public enum DeliveryStatus
{
    Pending = 0,
    Booked = 1,
    SentViaPaxi = 2,
    ReadyForCollection = 3,
    Collected = 4,
    IssueReported = 5
}

