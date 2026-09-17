import type { OrderingStep } from "../../data/storefront";

type OrderingStepCardProps = {
  step: OrderingStep;
};

export function OrderingStepCard({ step }: OrderingStepCardProps) {
  return (
    <article className="step-card">
      <span>{step.number}</span>
      <h3>{step.title}</h3>
      <p>{step.description}</p>
    </article>
  );
}
