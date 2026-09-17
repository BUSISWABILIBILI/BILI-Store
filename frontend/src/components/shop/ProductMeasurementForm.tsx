import { useMutation } from "@tanstack/react-query";
import { CheckCircle2, Palette, Ruler, Save, ShoppingBag, UserRound } from "lucide-react";
import type { CSSProperties } from "react";
import { useEffect, useMemo, useState } from "react";
import { validateProductMeasurements } from "../../services/catalogueApi";
import { useCartStore } from "../../stores/cartStore";
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
  const addItem = useCartStore((state) => state.addItem);
  const [values, setValues] = useState<Record<string, string>>({});
  const [profileName, setProfileName] = useState("My Measurements");
  const [profiles, setProfiles] = useState<SavedMeasurementProfile[]>(() => loadMeasurementProfiles());
  const [saveMessage, setSaveMessage] = useState("");
  const [colourMode, setColourMode] = useState<"swatch" | "custom">("swatch");
  const [selectedColourId, setSelectedColourId] = useState(product.availableColours[0]?.id ?? "");
  const [customColourName, setCustomColourName] = useState("");
  const [customColourHex, setCustomColourHex] = useState("#7a2d2a");
  const [quantity, setQuantity] = useState(1);
  const [cartMessage, setCartMessage] = useState("");

  useEffect(() => {
    setValues({});
    setSaveMessage("");
    setCartMessage("");
    setColourMode("swatch");
    setSelectedColourId(product.availableColours[0]?.id ?? "");
    setCustomColourName("");
    setCustomColourHex("#7a2d2a");
    setQuantity(1);
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
    setCartMessage("");
    validationMutation.reset();
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
    setCartMessage("");
    validationMutation.mutate({
      productSlug: product.slug,
      measurements: requiredMeasurements.map((measurement) => ({
        measurementKey: measurement.key,
        valueCm: toNumberOrNull(values[measurement.key]),
      })),
    });
  }

  function addProductToCart() {
    if (!validationMutation.data?.isValid) {
      setCartMessage("Validate the measurements before adding this custom garment to the cart.");
      return;
    }

    if (colourMode === "swatch" && !selectedColourId) {
      setCartMessage("Choose a colour before adding this custom garment to the cart.");
      return;
    }

    if (colourMode === "custom" && !customColourName.trim()) {
      setCartMessage("Enter a custom colour name before adding this custom garment to the cart.");
      return;
    }

    const selectedColour =
      colourMode === "swatch"
        ? product.availableColours.find((colour) => colour.id === selectedColourId) ?? null
        : null;

    addItem({
      productId: product.id,
      productSlug: product.slug,
      productName: product.name,
      productDescription: product.description,
      clothingType: product.clothingType,
      imageUrl: product.primaryImageUrl,
      imageAltText: product.primaryImageAltText,
      unitPrice: product.price,
      productionDaysEstimate: product.estimatedProductionDays,
      quantity,
      selectedColour,
      customColourName: colourMode === "custom" ? customColourName.trim() : null,
      customColourHex: colourMode === "custom" ? customColourHex : null,
      measurements: validationMutation.data.acceptedMeasurements,
    });

    setCartMessage(`${product.name} was added to your cart with measurements attached.`);
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

      <div className="customisation-grid">
        <section className="customisation-panel" aria-label="Colour selection">
          <div>
            <p className="eyebrow">Colour</p>
            <h4>Choose a swatch or custom colour</h4>
          </div>
          <div className="colour-choice-row">
            {product.availableColours.map((colour) => (
              <button
                className={
                  colourMode === "swatch" && selectedColourId === colour.id
                    ? "colour-choice colour-choice-active"
                    : "colour-choice"
                }
                key={colour.id}
                onClick={() => {
                  setColourMode("swatch");
                  setSelectedColourId(colour.id);
                  setCartMessage("");
                }}
                style={{ "--choice-colour": colour.hexCode } as CSSProperties}
                type="button"
              >
                <span aria-hidden="true" />
                {colour.name}
              </button>
            ))}
          </div>
          {product.allowsCustomColor ? (
            <div className="custom-colour-fields">
              <button
                className={colourMode === "custom" ? "pill-button pill-button-active" : "pill-button"}
                onClick={() => {
                  setColourMode("custom");
                  setCartMessage("");
                }}
                type="button"
              >
                <Palette aria-hidden="true" size={17} />
                Use custom colour
              </button>
              {colourMode === "custom" ? (
                <div className="custom-colour-inputs">
                  <label>
                    <span>Colour name</span>
                    <input
                      onChange={(event) => setCustomColourName(event.target.value)}
                      placeholder="e.g. Emerald green"
                      type="text"
                      value={customColourName}
                    />
                  </label>
                  <label>
                    <span>Colour picker</span>
                    <input
                      onChange={(event) => setCustomColourHex(event.target.value)}
                      type="color"
                      value={customColourHex}
                    />
                  </label>
                </div>
              ) : null}
            </div>
          ) : null}
        </section>

        <section className="customisation-panel" aria-label="Quantity">
          <div>
            <p className="eyebrow">Quantity</p>
            <h4>Set garment quantity</h4>
          </div>
          <label className="quantity-field">
            <span>Quantity</span>
            <input
              min={1}
              max={20}
              onChange={(event) => setQuantity(Math.max(1, Number(event.target.value)))}
              type="number"
              value={quantity}
            />
          </label>
        </section>
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
        <button className="button button-primary" onClick={addProductToCart} type="button">
          <ShoppingBag aria-hidden="true" size={18} />
          Add custom garment to cart
        </button>
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
      {cartMessage ? <p className="profile-note">{cartMessage}</p> : null}

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
