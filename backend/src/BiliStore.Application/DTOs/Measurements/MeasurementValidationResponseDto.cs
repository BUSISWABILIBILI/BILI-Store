namespace BiliStore.Application.DTOs.Measurements;

public sealed record MeasurementValidationResponseDto(
    bool IsValid,
    IReadOnlyList<MeasurementValidationIssueDto> Issues,
    IReadOnlyList<AcceptedMeasurementDto> AcceptedMeasurements);
