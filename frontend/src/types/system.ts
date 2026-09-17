export type HealthCheckResponse = {
  service: string;
  status: string;
  environment: string;
  serverTime: string;
};

export type ApiInfoResponse = {
  name: string;
  version: string;
  description: string;
  currentPhase: string;
};

