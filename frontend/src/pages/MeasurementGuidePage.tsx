import { Ruler } from "lucide-react";
import { SectionHeader } from "../components/ui/SectionHeader";
import { measurementGuideItems } from "../data/storefront";

export function MeasurementGuidePage() {
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
          description="The placeholders below are intentionally structured so final diagrams or photos can be dropped in later without rewriting the page."
        />
        <div className="measurement-grid">
          {measurementGuideItems.map((item) => (
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
