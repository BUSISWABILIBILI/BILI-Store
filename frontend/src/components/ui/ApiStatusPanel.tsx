import { useQuery } from "@tanstack/react-query";
import { getApiHealth, getApiInfo } from "../../services/systemApi";

export function ApiStatusPanel() {
  const healthQuery = useQuery({
    queryKey: ["api-health"],
    queryFn: getApiHealth,
  });

  const infoQuery = useQuery({
    queryKey: ["api-info"],
    queryFn: getApiInfo,
  });

  const health = healthQuery.data;
  const isHealthy = health?.status === "Healthy";

  return (
    <section className="api-panel" aria-label="API connection status">
      <div>
        <p className="eyebrow">Backend connection</p>
        <h2>{isHealthy ? "API is connected" : "Waiting for the API"}</h2>
        <p>
          {isHealthy
            ? `${health.service} is running in ${health.environment}.`
            : "Start the ASP.NET Core backend to see live status here."}
        </p>
      </div>
      <dl className="status-grid">
        <div>
          <dt>Status</dt>
          <dd>{healthQuery.isLoading ? "Checking" : health?.status ?? "Unavailable"}</dd>
        </div>
        <div>
          <dt>Phase</dt>
          <dd>{infoQuery.data?.currentPhase ?? "Phase 2 - Project setup"}</dd>
        </div>
        <div>
          <dt>API version</dt>
          <dd>{infoQuery.data?.version ?? "0.2.0"}</dd>
        </div>
      </dl>
    </section>
  );
}
