namespace BiliStore.Application.DTOs.Measurements;

public sealed record MeasurementValidationIssueDto(
    string MeasurementKey,
    string MeasurementName,
    string Message);
