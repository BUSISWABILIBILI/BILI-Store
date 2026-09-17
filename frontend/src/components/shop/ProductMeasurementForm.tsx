import { useMutation } from "@tanstack/react-query";
import { CheckCircle2, Ruler, Save, UserRound } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { validateProductMeasurements } from "../../services/catalogueApi";
import type { ProductDetails } from "../../types/catalogue";
import type { SavedMeasurementProfile } from "../../types/measurements";
import {
  createMeasurementProfile,
  loadMeasurementProfiles,
  saveMeasurementProfiles,
} from "../../utils/measurementProfiles";

type ProductMeasurementFormProps = {
  product: ProductDetails;
};

export function ProductMeasurementForm({ product }: ProductMeasurementFormProps) {
  const [values, setValues] = useState<Record<string, string>>({});
  const [profileName, setProfileName] = useState("My Measurements");
  const [profiles, setProfiles] = useState<SavedMeasurementProfile[]>(() => loadMeasurementProfiles());
  const [saveMessage, setSaveMessage] = useState("");

  useEffect(() => {
    setValues({});
    setSaveMessage("");
  }, [product.slug]);

  const requiredMeasurements = useMemo(
    () => product.measurementRequirements.filter((measurement) => measurement.isRequired),
    [product.measurementRequirements],
  );

  const validationMutation = useMutation({
    mutationFn: validateProductMeasurements,
  });

  function updateMeasurement(measurementKey: string, value: string) {
    setValues((currentValues) => ({
      ...currentValues,
      [measurementKey]: value,
    }));
  }

  function applyProfile(profile: SavedMeasurementProfile) {
    setValues((currentValues) => ({
      ...currentValues,
      ...profile.values,
    }));
    setSaveMessage(`${profile.name} loaded into this product.`);
    validationMutation.reset();
  }

  function saveProfile() {
    const cleanValues = Object.fromEntries(
      Object.entries(values).filter(([, value]) => value.trim().length > 0),
    );

    if (Object.keys(cleanValues).length === 0) {
      setSaveMessage("Enter at least one measurement before saving a profile.");
      return;
    }

    const nextProfile = createMeasurementProfile(profileName.trim() || "My Measurements", cleanValues);
    const nextProfiles = [
      nextProfile,
      ...profiles.filter((profile) => profile.name.toLowerCase() !== nextProfile.name.toLowerCase()),
    ].slice(0, 5);

    setProfiles(nextProfiles);
    saveMeasurementProfiles(nextProfiles);
    setSaveMessage(`${nextProfile.name} saved on this device.`);
  }

  function validateMeasurements() {
    validationMutation.mutate({
      productSlug: product.slug,
      measurements: requiredMeasurements.map((measurement) => ({
        measurementKey: measurement.key,
        valueCm: toNumberOrNull(values[measurement.key]),
      })),
    });
  }

  return (
    <section className="measurement-workspace" aria-label="Product measurement form">
      <div className="measurement-workspace-header">
        <div>
          <p className="eyebrow">Required measurements</p>
          <h3>Enter measurements for {product.name}</h3>
          <p>
            These fields come from the product detail API. The backend checks missing values and
            centimetre ranges before the item can move into the cart flow.
          </p>
        </div>
        <Ruler aria-hidden="true" size={34} />
      </div>

      <div className="measurement-form-grid">
        {requiredMeasurements.map((measurement) => {
          const fieldId = `${product.slug}-${measurement.key}`;

          return (
            <label className="measurement-field" htmlFor={fieldId} key={measurement.id}>
              <span>
                {measurement.name}
                <small>{measurement.helpText}</small>
              </span>
              <div className="measurement-input-row">
                <input
                  id={fieldId}
                  inputMode="decimal"
                  min={measurement.minValueCm ?? 0}
                  max={measurement.maxValueCm ?? undefined}
                  onChange={(event) => updateMeasurement(measurement.key, event.target.value)}
                  placeholder="0"
                  step="0.5"
                  type="number"
                  value={values[measurement.key] ?? ""}
                />
                <span>{measurement.unit}</span>
              </div>
            </label>
          );
        })}
      </div>

      <div className="measurement-actions">
        <button className="button button-primary" onClick={validateMeasurements} type="button">
          <CheckCircle2 aria-hidden="true" size={18} />
          Validate measurements
        </button>
        <div className="save-profile-row">
          <label className="sr-only" htmlFor={`${product.slug}-profile-name`}>
            Profile name
          </label>
          <input
            id={`${product.slug}-profile-name`}
            onChange={(event) => setProfileName(event.target.value)}
            placeholder="Profile name"
            type="text"
            value={profileName}
          />
          <button className="button button-light" onClick={saveProfile} type="button">
            <Save aria-hidden="true" size={18} />
            Save profile
          </button>
        </div>
      </div>

      {validationMutation.isPending ? <p className="inline-alert">Checking measurements...</p> : null}

      {validationMutation.data?.isValid ? (
        <div className="success-panel" role="status">
          <CheckCircle2 aria-hidden="true" size={22} />
          <p>
            Measurements look good. {validationMutation.data.acceptedMeasurements.length} values are
            ready to travel with this product in the cart phase.
          </p>
        </div>
      ) : null}

      {validationMutation.data && !validationMutation.data.isValid ? (
        <div className="validation-panel" role="alert">
          <strong>Fix these measurements:</strong>
          <ul>
            {validationMutation.data.issues.map((issue) => (
              <li key={`${issue.measurementKey}-${issue.message}`}>{issue.message}</li>
            ))}
          </ul>
        </div>
      ) : null}

      {validationMutation.isError ? (
        <p className="inline-alert">The measurement validator is not reachable right now.</p>
      ) : null}

      {saveMessage ? <p className="profile-note">{saveMessage}</p> : null}

      {profiles.length > 0 ? (
        <div className="saved-profile-panel">
          <div>
            <UserRound aria-hidden="true" size={20} />
            <strong>Saved on this device</strong>
          </div>
          <div className="saved-profile-list">
            {profiles.map((profile) => (
              <button className="pill-button" key={profile.id} onClick={() => applyProfile(profile)} type="button">
                {profile.name}
              </button>
            ))}
          </div>
        </div>
      ) : null}
    </section>
  );
}

function toNumberOrNull(value: string | undefined) {
  if (!value) {
    return null;
  }

  const parsedValue = Number(value);

  return Number.isFinite(parsedValue) ? parsedValue : null;
}
