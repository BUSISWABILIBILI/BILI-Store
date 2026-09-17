export type MeasurementValueInput = {
  measurementKey: string;
  valueCm: number | null;
};

export type ValidateProductMeasurementsRequest = {
  productSlug: string;
  measurements: MeasurementValueInput[];
};

export type MeasurementValidationIssue = {
  measurementKey: string;
  measurementName: string;
  message: string;
};

export type AcceptedMeasurement = {
  measurementKey: string;
  measurementName: string;
  valueCm: number;
  unit: string;
};

export type MeasurementValidationResponse = {
  isValid: boolean;
  issues: MeasurementValidationIssue[];
  acceptedMeasurements: AcceptedMeasurement[];
};

export type SavedMeasurementProfile = {
  id: string;
  name: string;
  values: Record<string, string>;
  updatedAt: string;
};
