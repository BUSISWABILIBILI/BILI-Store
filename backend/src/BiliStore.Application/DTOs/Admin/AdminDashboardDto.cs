namespace BiliStore.Application.DTOs.Admin;

public sealed record AdminDashboardDto(
    int OrderCount,
    int PendingOrderCount,
    int InProductionOrderCount,
    int CompletedOrderCount,
    decimal PaidRevenue,
    decimal PendingRevenue,
    int CustomerCount,
    int GuestOrderCount,
    int AvailableProductCount,
    int UnavailableProductCount,
    int ReturnRequestCount,
    int OpenReturnRequestCount,
    IReadOnlyList<AdminDashboardStatusCountDto> OrdersByStatus,
    IReadOnlyList<AdminDashboardStatusCountDto> PaymentsByStatus,
    IReadOnlyList<AdminDashboardRecentOrderDto> RecentOrders,
    IReadOnlyList<AdminDashboardProductAlertDto> ProductAlerts);
