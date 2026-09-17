import { useQuery } from "@tanstack/react-query";
import { Ruler } from "lucide-react";
import { SectionHeader } from "../components/ui/SectionHeader";
import { measurementGuideItems } from "../data/storefront";
import { getMeasurements } from "../services/catalogueApi";

export function MeasurementGuidePage() {
  const measurementsQuery = useQuery({
    queryKey: ["measurements"],
    queryFn: getMeasurements,
  });
  const guideItems =
    measurementsQuery.data && measurementsQuery.data.length > 0
      ? measurementsQuery.data.map((measurement) => ({
          name: measurement.name,
          summary: measurement.description,
          steps: [measurement.instructions],
          tip: measurement.tip,
        }))
      : measurementGuideItems;

  return (
    <div className="stack-xl">
      <section className="guide-hero">
        <div>
          <p className="eyebrow">Custom fit foundation</p>
          <h1>How to Measure Yourself</h1>
          <p>
            Use a soft measuring tape, keep it level, and measure in centimetres. These guide
            cards prepare the UI for Phase 5, where required measurements will come from the
            backend by garment type.
          </p>
        </div>
        <div className="guide-figure" aria-hidden="true">
          <Ruler size={58} />
          <span>cm</span>
        </div>
      </section>

      <section>
        <SectionHeader
          eyebrow="Measurement instructions"
          title="Clear, repeatable steps reduce ordering mistakes."
          description="These cards now load measurement definitions from the backend when the API is running."
        />
        {measurementsQuery.isError ? (
          <p className="inline-alert">
            Measurement cards are using local preview copy because the API is not reachable.
          </p>
        ) : null}
        <div className="measurement-grid">
          {guideItems.map((item) => (
            <article className="measurement-card" key={item.name}>
              <div className="measurement-illustration" aria-hidden="true">
                <Ruler size={30} />
              </div>
              <div>
                <h3>{item.name}</h3>
                <p>{item.summary}</p>
                <ol>
                  {item.steps.map((step) => (
                    <li key={step}>{step}</li>
                  ))}
                </ol>
                <strong>{item.tip}</strong>
              </div>
            </article>
          ))}
        </div>
      </section>
    </div>
  );
}
